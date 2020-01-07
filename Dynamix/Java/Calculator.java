package com.xamarin.dynamix;

public class Calculator {
    public int add(int first, int second) {
        return first + second;
    }

    public void add(int first, int second, AnswerListener listener) {
        listener.onCalculated(first + second);
    }
}
