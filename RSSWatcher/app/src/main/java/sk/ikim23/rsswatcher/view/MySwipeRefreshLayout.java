package sk.ikim23.rsswatcher.view;

import android.content.Context;
import android.support.v4.widget.SwipeRefreshLayout;
import android.util.AttributeSet;
import android.util.Log;

import java.util.concurrent.atomic.AtomicBoolean;

public class MySwipeRefreshLayout extends SwipeRefreshLayout {

    private AtomicBoolean isEnabled;

    public MySwipeRefreshLayout(Context context) {
        super(context);
    }

    public MySwipeRefreshLayout(Context context, AttributeSet attributeSet) {
        super(context, attributeSet);
    }

    @Override
    public void setEnabled(boolean enabled) {
        super.setEnabled(enabled);
        if (isEnabled == null) {
            isEnabled = new AtomicBoolean();
            Log.d("my view", "null ref set");
        }
        isEnabled.set(enabled);
        Log.d("my view", "set: " + enabled);
    }

    public void trySetEnabled(boolean enabled) {
        if (isEnabled == null) {
            isEnabled = new AtomicBoolean(isEnabled());
            Log.d("my view", "null ref tryset ");
        }
        if (isEnabled.get()) {
            super.setEnabled(enabled);
        }
        Log.d("my view", "tryset: " + enabled);
    }
}
