OS           ?= $(shell uname)

V             ?= 0
CONFIGURATION = Debug

ifeq ($(OS),Darwin)
NATIVE_EXT = .dylib
DLLMAP_OS_NAME = osx
endif
ifeq ($(OS),Linux)
NATIVE_EXT = .so
DLLMAP_OS_NAME = linux
endif

DEPENDENCIES = \
	bin/Test$(CONFIGURATION)/libNativeTiming$(NATIVE_EXT)

TESTS = \
	bin/Test$(CONFIGURATION)/Java.Interop-Tests.dll \
	bin/Test$(CONFIGURATION)/Java.Interop.Dynamic-Tests.dll \
	bin/Test$(CONFIGURATION)/Java.Interop.Export-Tests.dll \
	bin/Test$(CONFIGURATION)/Java.Interop.Tools.JavaCallableWrappers-Tests.dll \
	bin/Test$(CONFIGURATION)/Java.Interop.Tools.JavaSource-Tests.dll \
	bin/Test$(CONFIGURATION)/logcat-parse-Tests.dll \
	bin/Test$(CONFIGURATION)/generator-Tests.dll \
	bin/Test$(CONFIGURATION)/Xamarin.Android.Tools.ApiXmlAdjuster-Tests.dll \
	bin/Test$(CONFIGURATION)/Xamarin.Android.Tools.Bytecode-Tests.dll

PTESTS = \
	bin/Test$(CONFIGURATION)/Java.Interop-PerformanceTests.dll

ATESTS = \
	bin/Test$(CONFIGURATION)/Android.Interop-Tests.dll

BUILD_PROPS = bin/Build$(CONFIGURATION)/MonoInfo.props

all: 
	$(MSBUILD) $(MSBUILD_FLAGS) /restore Java.Interop.sln
  
run-all-tests:
	r=0; \
	$(MAKE) run-tests                 || r=1 ; \
	$(MAKE) run-test-jnimarshal       || r=1 ; \
	$(MAKE) run-test-generator-core   || r=1 ; \
	exit $$r;

include build-tools/scripts/msbuild.mk

prepare:: $(BUILD_PROPS) src/Java.Runtime.Environment/Java.Runtime.Environment.dll.config

prepare:: 
	$(MSBUILD) $(MSBUILD_FLAGS) /t:Restore Java.Interop.sln

clean:
	-$(MSBUILD) $(MSBUILD_FLAGS) /t:Clean
	-rm -Rf bin/$(CONFIGURATION) bin/Build$(CONFIGURATION) bin/Test$(CONFIGURATION)
	-rm src/Java.Runtime.Environment/Java.Runtime.Environment.dll.config

include build-tools/scripts/mono.mk
include build-tools/scripts/jdk.mk

JAVA_RUNTIME_ENVIRONMENT_DLLMAP_OVERRIDE = Java.Runtime.Environment.Override.dllmap
ifeq ($(wildcard $(JAVA_RUNTIME_ENVIRONMENT_DLLMAP_OVERRIDE)),)
	JAVA_RUNTIME_ENVIRONMENT_DLLMAP_OVERRIDE_CMD = '/@JAVA_RUNTIME_ENVIRONMENT_DLLMAP@/d'
else
	JAVA_RUNTIME_ENVIRONMENT_DLLMAP_OVERRIDE_CMD = '/@JAVA_RUNTIME_ENVIRONMENT_DLLMAP@/ {' -e 'r $(JAVA_RUNTIME_ENVIRONMENT_DLLMAP_OVERRIDE)' -e 'd' -e '}'
endif

src/Java.Runtime.Environment/Java.Runtime.Environment.dll.config: src/Java.Runtime.Environment/Java.Runtime.Environment.dll.config.in \
		bin/Build$(CONFIGURATION)/JdkInfo.props
	sed -e 's#@JI_JVM_PATH@#$(JI_JVM_PATH)#g' -e 's#@OS_NAME@#$(DLLMAP_OS_NAME)#g' -e $(JAVA_RUNTIME_ENVIRONMENT_DLLMAP_OVERRIDE_CMD) < $< > $@

JAVA_INTEROP_LIB    = libjava-interop$(NATIVE_EXT)
NATIVE_TIMING_LIB   = libNativeTiming$(NATIVE_EXT)

bin/Test$(CONFIGURATION)/$(NATIVE_TIMING_LIB): tests/NativeTiming/timing.c $(wildcard $(JI_JDK_INCLUDE_PATHS)/jni.h)
	mkdir -p `dirname "$@"`
	gcc -g -shared -m64 -fPIC -o $@ $< $(JI_JDK_INCLUDE_PATHS:%=-I%)

# Usage: $(call TestAssemblyTemplate,assembly-basename)
define TestAssemblyTemplate
bin/Test$$(CONFIGURATION)/$(1)-Tests.dll: $(wildcard src/$(1)/*/*.cs src/$(1)/Test*/*/*.cs)
	$$(MSBUILD) $$(MSBUILD_FLAGS)
	touch $$@
endef # TestAssemblyTemplate

$(eval $(call TestAssemblyTemplate,Java.Interop))
$(eval $(call TestAssemblyTemplate,Java.Interop.Dynamic))
$(eval $(call TestAssemblyTemplate,Java.Interop.Export))
$(eval $(call TestAssemblyTemplate,Java.Interop.Tools.JavaCallableWrappers))

bin/Test$(CONFIGURATION)/Java.Interop-PerformanceTests.dll: $(wildcard tests/Java.Interop-PerformanceTests/*.cs) bin/Test$(CONFIGURATION)/$(NATIVE_TIMING_LIB)
	$(MSBUILD) $(MSBUILD_FLAGS)
	touch $@

bin/Test$(CONFIGURATION)/Android.Interop-Tests.dll: $(wildcard src/Android.Interop/*/*.cs src/Android.Interop/Tests/*/*.cs)
	$(MSBUILD) $(MSBUILD_FLAGS)
	touch $@

bin/$(CONFIGURATION)/Java.Interop.dll: $(wildcard src/Java.Interop/*/*.cs) src/Java.Interop/Java.Interop.csproj
	$(MSBUILD) $(if $(V),/v:diag,) /p:Configuration=$(CONFIGURATION) $(if $(SNK),"/p:AssemblyOriginatorKeyFile=$(SNK)",)

CSHARP_REFS = \
	bin/$(CONFIGURATION)/Java.Interop.dll               \
	bin/$(CONFIGURATION)/Java.Interop.Export.dll        \
	bin/$(CONFIGURATION)/Java.Runtime.Environment.dll   \
	bin/Test$(CONFIGURATION)/TestJVM.dll                    \
	$(PTESTS)                                           \
	$(TESTS)

shell:
	MONO_TRACE_LISTENER=Console.Out \
	MONO_OPTIONS=--debug=casts csharp $(patsubst %,-r:%,$(CSHARP_REFS))

# $(call RUN_TEST,filename,log-lref?)
define RUN_TEST
	$(MSBUILD) $(MSBUILD_FLAGS) build-tools/scripts/RunNUnitTests.targets /p:TestAssembly=$(1) || r=1;
endef

run-tests: $(TESTS) bin/Test$(CONFIGURATION)/$(JAVA_INTEROP_LIB)
	r=0; \
	$(foreach t,$(TESTS), $(call RUN_TEST,$(t),1)) \
	exit $$r;

run-ptests: $(PTESTS) bin/Test$(CONFIGURATION)/$(JAVA_INTEROP_LIB)
	r=0; \
	$(foreach t,$(PTESTS), $(call RUN_TEST,$(t))) \
	exit $$r;

bin/Test$(CONFIGURATION)/$(JAVA_INTEROP_LIB): bin/$(CONFIGURATION)/$(JAVA_INTEROP_LIB)
	cp $< $@

JRE_DLL_CONFIG=bin/$(CONFIGURATION)/Java.Runtime.Environment.dll.config

$(JRE_DLL_CONFIG): src/Java.Runtime.Environment/Java.Runtime.Environment.csproj
	$(MSBUILD) $(MSBUILD_FLAGS) $<

define run-jnimarshalmethod-gen
	MONO_TRACE_LISTENER=Console.Out \
	$(RUNTIME) bin/$(CONFIGURATION)/jnimarshalmethod-gen.exe -v --jvm "$(JI_JVM_PATH)" -L "$(JI_MONO_LIB_PATH)mono/4.5" -L "$(JI_MONO_LIB_PATH)mono/4.5/Facades" $(2) $(1)
endef

run-test-jnimarshal: bin/Test$(CONFIGURATION)/Java.Interop.Export-Tests.dll bin/Test$(CONFIGURATION)/$(JAVA_INTEROP_LIB) $(JRE_DLL_CONFIG)
	mkdir -p test-jni-output
	$(call run-jnimarshalmethod-gen,"$<",-f -o test-jni-output --keeptemp)
	(test -f test-jni-output/$(notdir $<) && test -f test-jni-output/Java.Interop.Export-Tests-JniMarshalMethods.dll) || { echo "jnimarshalmethod-gen did not create the expected assemblies in the test-jni-output directory"; exit 1; }
	$(call run-jnimarshalmethod-gen,"$<")
	$(call RUN_TEST,$<)

# $(call GEN_CORE_OUTPUT, outdir, suffix, extra)
define GEN_CORE_OUTPUT
	-$(RM) -Rf $(1)
	mkdir -p $(1)
	$(RUNTIME) bin/Test$(CONFIGURATION)/generator.exe -o $(1) $(3) --api-level=20 tests/generator-Tests/Tests-Core/api$(2).xml \
		--enummethods=tests/generator-Tests/Tests-Core/methods$(2).xml \
		--enumfields=tests/generator-Tests/Tests-Core/fields$(2).xml \
		--enumdir=$(1)
endef

run-test-generator-core: bin/Test$(CONFIGURATION)/generator.exe
	$(call GEN_CORE_OUTPUT,bin/Test$(CONFIGURATION)/generator-core)
	diff -rup --strip-trailing-cr tests/generator-Tests/Tests-Core/expected bin/Test$(CONFIGURATION)/generator-core
	$(call GEN_CORE_OUTPUT,bin/Test$(CONFIGURATION)/generator-core,,--codegen-target=JavaInterop1)
	diff -rup --strip-trailing-cr tests/generator-Tests/Tests-Core/expected.ji bin/Test$(CONFIGURATION)/generator-core
	$(call GEN_CORE_OUTPUT,bin/Test$(CONFIGURATION)/generator-core,-cp)
	diff -rup --strip-trailing-cr tests/generator-Tests/Tests-Core/expected.cp bin/Test$(CONFIGURATION)/generator-core

bin/Test$(CONFIGURATION)/generator.exe: bin/$(CONFIGURATION)/generator.exe
	cp $<* `dirname "$@"`

update-test-generator-core:
	$(call GEN_CORE_OUTPUT,tests/generator-Tests/Tests-Core/expected)
	$(call GEN_CORE_OUTPUT,tests/generator-Tests/Tests-Core/expected.ji,--codegen-target=JavaInterop1)

update-test-generator-nunit:
	-$(MAKE) run-tests TESTS=bin/Test$(CONFIGURATION)/generator-Tests.dll
	for f in `find tests/generator-Tests/expected -name \*.cs` ; do \
		source=`echo $$f | sed 's#^tests/generator-Tests/expected#bin/Test$(CONFIGURATION)/out#'` ; \
		if [ -f "$$source" ]; then \
			cp -f "$$source" "$$f" ; \
		fi; \
	done
	for source in `find bin/Test$(CONFIGURATION)/out.ji -type f` ; do \
		f=`echo $$source | sed 's#^bin/Test$(CONFIGURATION)/out.ji#tests/generator-Tests/expected.ji#'` ; \
		mkdir -p `dirname $$f`; \
		cp -f "$$source" "$$f" ; \
	done
