package sk.ikim23.rsswatcher.activity;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.net.ConnectivityManager;

import static android.net.NetworkInfo.State;

import android.net.NetworkInfo;
import android.os.AsyncTask;
import android.support.v4.content.ContextCompat;
import android.view.View;
import android.widget.TextView;

import java.util.concurrent.Executors;
import java.util.concurrent.TimeUnit;

import sk.ikim23.rsswatcher.R;
import sk.ikim23.rsswatcher.U;

public class ConnectivityReceiver extends BroadcastReceiver {

    private static State previousState = State.CONNECTED;
    private final TextView networkStatus;
    private boolean firstReceive = true;
    private boolean isEnabled = true;

    public ConnectivityReceiver(TextView networkStatus) {
        this.networkStatus = networkStatus;
    }

    public void checkConnectivity(Context context) {
        ConnectivityManager cm = (ConnectivityManager) context.getSystemService(Context.CONNECTIVITY_SERVICE);
        NetworkInfo networkInfo = cm.getActiveNetworkInfo();
        receive(context, networkInfo);
    }

    @Override
    public void onReceive(Context context, Intent intent) {
        if (!firstReceive) {
            if (intent != null) {
                NetworkInfo networkInfo = intent.getParcelableExtra("networkInfo");
                receive(context, networkInfo);
            }
        } else {
            firstReceive = false;
        }
    }

    public void onConnected(Context context) {
        networkStatus.setBackgroundColor(ContextCompat.getColor(context, R.color.network_status_green));
        networkStatus.setText(R.string.network_status_connected);
        networkStatus.setVisibility(View.VISIBLE);
        hideStatus(2);
    }

    public void onConnecting(Context context) {
        networkStatus.setBackgroundColor(ContextCompat.getColor(context, R.color.network_status_orange));
        networkStatus.setText(R.string.network_status_connecting);
        networkStatus.setVisibility(View.VISIBLE);
    }

    public void onDisconnected(Context context) {
        showErrorMessage(context, R.string.network_status_disconnected);
    }

    public void showErrorMessage(Context context, int resId) {
        networkStatus.setBackgroundColor(ContextCompat.getColor(context, R.color.network_status_red));
        networkStatus.setText(resId);
        networkStatus.setVisibility(View.VISIBLE);
        hideStatus(2);
    }

    private void receive(Context context, NetworkInfo networkInfo) {
        if (isEnabled) {
            if (networkInfo == null || networkInfo.getState() == State.DISCONNECTED) {
                previousState = State.DISCONNECTED;
                onDisconnected(context);
            } else if (networkInfo.getState() == State.CONNECTING) {
                previousState = State.CONNECTING;
                onConnecting(context);
            } else if (networkInfo.getState() == State.CONNECTED) {
                if (U.getPrefOnlyWifi(context) && networkInfo.getType() != ConnectivityManager.TYPE_WIFI) {
                    previousState = State.DISCONNECTED;
                    onDisconnected(context);
                } else {
                    if (previousState != State.CONNECTED) {
                        previousState = State.CONNECTED;
                        onConnected(context);
                    }
                }
            }
        }
    }

    private void hideStatus(int sec) {
        Executors.newScheduledThreadPool(1).schedule(new Runnable() {
            @Override
            public void run() {
                new AsyncTask<Void, Void, Void>() {
                    @Override
                    protected Void doInBackground(Void... v) {
                        return null;
                    }

                    @Override
                    protected void onPostExecute(Void y) {
                        networkStatus.setVisibility(View.GONE);
                    }
                }.execute();
            }
        }, sec, TimeUnit.SECONDS);
    }

    public void setEnabled(boolean enabled) {
        this.isEnabled = enabled;
    }
}
