package sk.ikim23.rsswatcher.service;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;

import sk.ikim23.rsswatcher.U;

import static android.net.NetworkInfo.State.CONNECTED;

public class ConnectivityChangeReceiver extends BroadcastReceiver {

    @Override
    public void onReceive(Context context, Intent intent) {
        if (intent != null && ConnectivityManager.CONNECTIVITY_ACTION.equals(intent.getAction())) {
            NetworkInfo networkInfo = intent.getParcelableExtra("networkInfo");
            if (networkInfo != null && networkInfo.getState() == CONNECTED) {
                Intent serviceIntent = new Intent(context, DownloadService.class);
                serviceIntent.putExtra(U.EXTRA_CALLER, getClass().getName());
                serviceIntent.putExtra(U.EXTRA_SCHEDULE_NEXT_SERVICE, U.scheduleServiceIfNotBefore());
                context.startService(serviceIntent);
            }
        }
    }
}
