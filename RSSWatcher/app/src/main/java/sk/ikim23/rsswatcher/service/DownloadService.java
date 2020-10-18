package sk.ikim23.rsswatcher.service;

import android.app.AlarmManager;
import android.app.PendingIntent;
import android.app.Service;
import android.content.Intent;
import android.net.wifi.WifiManager;
import android.os.IBinder;
import android.os.PowerManager;
import android.os.SystemClock;
import android.support.annotation.Nullable;
import android.util.Log;

import sk.ikim23.rsswatcher.U;

public class DownloadService extends Service {

    private static final String WAKE_LOCK_TAG = "wake_lock";
    private static final String WIFI_LOCK_TAG = "wifi_lock";
    private WifiManager.WifiLock wifiLock;
    private PowerManager.WakeLock wakeLock;

    @Override
    public int onStartCommand(Intent intent, int flags, int startId) {
        Log.d(getClass().getName(), "Starting onStartCommand");
        if (intent == null) {
            Log.d(getClass().getName(), "intent is null");
            return START_STICKY;
        }
        String caller = intent.getStringExtra(U.EXTRA_CALLER);
        Log.d(getClass().getName(), "Caller: " + caller);

        lockCPU();
        lockWifi();
        if (U.isConnected(this)) {
            long channelId = intent.getLongExtra(U.EXTRA_CHANNEL_ID, U.CHANNEL_ID_UNDEFINED);
            new FeedDownloadTask(this, channelId).execute();
        } else {
            Log.d(getClass().getName(), "No internet connection");
        }

        boolean scheduleNextService = intent.getBooleanExtra(U.EXTRA_SCHEDULE_NEXT_SERVICE, false);
        if (scheduleNextService) {
            Log.d(getClass().getName(), "Scheduling new service");
            int min = U.getPrefSyncFrequency(this);
            if (min != U.getPrefSyncFrequencyNever(this)) {
                AlarmManager alarmManager = (AlarmManager) getSystemService(ALARM_SERVICE);
                Intent serviceIntent = new Intent(this, DownloadService.class);
                serviceIntent.putExtra(U.EXTRA_CALLER, getClass().getName());
                serviceIntent.putExtra(U.EXTRA_SCHEDULE_NEXT_SERVICE, true);
                Log.d(getClass().getName(), "Next service will run in " + min + " minutes");
                alarmManager.set(
                        AlarmManager.ELAPSED_REALTIME_WAKEUP,
                        SystemClock.elapsedRealtime() + U.minToMillis(min),
                        PendingIntent.getService(this, 10, serviceIntent, 0)
                );
            } else {
                Log.d(getClass().getName(), "Scheduling service is turned off");
            }
        }
        Log.d(getClass().getName(), "Ending onStartCommand");
        return START_STICKY;
    }

    @Override
    public void onDestroy() {
        super.onDestroy();
        if (wifiLock != null && wifiLock.isHeld()) {
            wifiLock.release();
        }
        if (wakeLock != null && wakeLock.isHeld()) {
            wakeLock.release();
        }
    }

    @Nullable
    @Override
    public IBinder onBind(Intent intent) {
        return null;
    }

    private void lockWifi() {
        if (wifiLock == null || !wifiLock.isHeld()) {
            WifiManager wifiManager = (WifiManager) getSystemService(WIFI_SERVICE);
            wifiLock = wifiManager.createWifiLock(WifiManager.WIFI_MODE_FULL, WIFI_LOCK_TAG);
            wifiLock.acquire();
            Log.d(getClass().getName(), "wifi lock acquired: " + wifiLock.isHeld());
        }
    }

    private void lockCPU() {
        if (wakeLock == null || !wakeLock.isHeld()) {
            PowerManager powerManager = (PowerManager) getSystemService(POWER_SERVICE);
            wakeLock = powerManager.newWakeLock(PowerManager.PARTIAL_WAKE_LOCK, WAKE_LOCK_TAG);
            wakeLock.acquire();
            Log.d(getClass().getName(), "wake lock acquired: " + wakeLock.isHeld());
        }
    }

}
