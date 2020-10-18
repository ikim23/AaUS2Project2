package sk.ikim23.rsswatcher.activity.drawer;

import android.content.Context;
import android.view.View;
import android.widget.TextView;

import sk.ikim23.rsswatcher.activity.ConnectivityReceiver;
import sk.ikim23.rsswatcher.view.MySwipeRefreshLayout;

public class ConnectivityRefreshReceiver extends ConnectivityReceiver {

    private final MySwipeRefreshLayout swipeRefreshLayout;

    public ConnectivityRefreshReceiver(TextView networkStatus, MySwipeRefreshLayout swipeRefreshLayout) {
        super(networkStatus);
        this.swipeRefreshLayout = swipeRefreshLayout;
    }

    @Override
    public void onConnected(Context context) {
        super.onConnected(context);
        if (swipeRefreshLayout != null) {
            swipeRefreshLayout.trySetEnabled(true);
            if (swipeRefreshLayout.getVisibility() == View.VISIBLE) {
                swipeRefreshLayout.setRefreshing(true);
            }
        }
    }

    @Override
    public void onDisconnected(Context context) {
        super.onDisconnected(context);
        if (swipeRefreshLayout != null) {
            if (swipeRefreshLayout.isRefreshing()) {
                swipeRefreshLayout.setRefreshing(false);
            }
            swipeRefreshLayout.trySetEnabled(false);
        }
    }
}