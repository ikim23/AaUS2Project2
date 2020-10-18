package sk.ikim23.rsswatcher.service;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;

import sk.ikim23.rsswatcher.U;

public class BootCompletedReceiver extends BroadcastReceiver {
    @Override
    public void onReceive(Context context, Intent intent) {
        if (intent != null && Intent.ACTION_BOOT_COMPLETED.equals(intent.getAction())) {
            Intent serviceIntent = new Intent(context, DownloadService.class);
            serviceIntent.putExtra(U.EXTRA_CALLER, getClass().getName());
            serviceIntent.putExtra(U.EXTRA_SCHEDULE_NEXT_SERVICE, U.scheduleServiceIfNotBefore());
            context.startService(serviceIntent);
        }
    }
}
