package sk.ikim23.rsswatcher.service;

import android.app.IntentService;
import android.content.Intent;
import android.net.Uri;
import android.util.Log;

import sk.ikim23.rsswatcher.U;
import sk.ikim23.rsswatcher.data.DbHelper;
import sk.ikim23.rsswatcher.data.FeedProvider;

public class NotificationDeleteService extends IntentService {

    public NotificationDeleteService() {
        super("NotificationDeleteService");
    }

    @Override
    protected void onHandleIntent(Intent intent) {
        Log.d(getClass().getName(), "service started");
        if (intent != null) {
            long filterId = intent.getLongExtra(U.EXTRA_FILTER_ID, U.FILTER_ID_DEFAULT);
            if (filterId != U.FILTER_ID_DEFAULT) {
                Uri uri = Uri.parse("content://" + FeedProvider.AUTHORITY + "/" + FeedProvider.PATH_NOTIFICATION);
                String where = DbHelper.NOTIFICATION_FILTER_ID + "=" + filterId;
                int deleteCount = getContentResolver().delete(uri, where, null);
                Log.d(getClass().getName(), deleteCount + " rows deleted");
            }
        }
    }

}
