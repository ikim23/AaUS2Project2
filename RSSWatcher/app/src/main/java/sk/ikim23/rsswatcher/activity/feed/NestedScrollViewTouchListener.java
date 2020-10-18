package sk.ikim23.rsswatcher.activity.feed;

import android.support.v4.widget.NestedScrollView;
import android.view.MotionEvent;
import android.view.View;

import java.util.concurrent.atomic.AtomicBoolean;

public class NestedScrollViewTouchListener implements View.OnTouchListener {

    private final AtomicBoolean preventScroll = new AtomicBoolean(false);
    private int scrollViewHeight;
    private int contentHeight;

    public NestedScrollViewTouchListener(NestedScrollView scrollView, View content) {
        init(scrollView, content);
    }

    @Override
    public boolean onTouch(View view, MotionEvent motionEvent) {
        return preventScroll.get();
    }

    private void init(NestedScrollView scrollView, View content) {
        scrollView.addOnLayoutChangeListener(new View.OnLayoutChangeListener() {
            @Override
            public void onLayoutChange(View v, int left, int top, int right, int bottom,
                                       int oldLeft, int oldTop, int oldRight, int oldBottom) {
                scrollViewHeight = bottom;
                preventScroll.set(scrollViewHeight > contentHeight);
            }
        });
        content.addOnLayoutChangeListener(new View.OnLayoutChangeListener() {
            @Override
            public void onLayoutChange(View v, int left, int top, int right, int bottom,
                                       int oldLeft, int oldTop, int oldRight, int oldBottom) {
                contentHeight = bottom;
                preventScroll.set(scrollViewHeight > contentHeight);
            }
        });
    }
}
