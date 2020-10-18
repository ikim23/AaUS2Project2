package sk.ikim23.rsswatcher.service;

import android.app.IntentService;
import android.content.ContentValues;
import android.content.Intent;
import android.database.Cursor;
import android.net.Uri;
import android.text.TextUtils;
import android.util.Log;

import java.util.LinkedList;

import sk.ikim23.rsswatcher.U;
import sk.ikim23.rsswatcher.data.DbHelper;
import sk.ikim23.rsswatcher.data.FeedProvider;

public class DeleteService extends IntentService {

    public DeleteService() {
        super("DeleteService");
    }

    @Override
    protected void onHandleIntent(Intent intent) {
        Log.d(getClass().getName(), "Starting service");
        Uri uri = Uri.parse("content://" + FeedProvider.AUTHORITY + "/" + FeedProvider.PATH_FEED);
        String selection = DbHelper.FEED_IS_ARCHIVED + " = 0";
        Cursor cursor = getContentResolver().query(uri, new String[]{DbHelper.FEED_ID, DbHelper.FEED_PUB_DATE}, selection, null, null, null);
        if (cursor != null) {
            int makDeletedMaxDays = U.getPrefDeleteFrequency(getBaseContext());
            Log.d(getClass().getName(), "Delete frequency is " + makDeletedMaxDays + " days");
            LinkedList<Long> markDeletedIds = new LinkedList<>();
            LinkedList<Long> deleteIds = new LinkedList<>();
            cursor.moveToFirst();
            while (!cursor.isAfterLast()) {
                long diffDays = (System.currentTimeMillis() - cursor.getLong(cursor.getColumnIndex(DbHelper.FEED_PUB_DATE))) / (24 * 60 * 60 * 1000);
                if (diffDays > makDeletedMaxDays) {
                    if (diffDays > U.DELETE_AFTER_DAYS) {
                        deleteIds.add(cursor.getLong(cursor.getColumnIndex(DbHelper.FEED_ID)));
                    } else {
                        markDeletedIds.add(cursor.getLong(cursor.getColumnIndex(DbHelper.FEED_ID)));
                    }
                }
                cursor.moveToNext();
            }
            cursor.close();

            if (markDeletedIds.size() > 0) {
                ContentValues values = new ContentValues();
                values.put(DbHelper.FEED_IS_DELETED, 1);
                String where = DbHelper.FEED_ID + " IN (" + TextUtils.join(",", markDeletedIds) + ")";
                getContentResolver().update(uri, values, where, null);
            }
            Log.d(getClass().getName(), markDeletedIds.size() + " feeds marked as deleted");
            if (deleteIds.size() > 0) {
                String where = DbHelper.FEED_ID + " IN (" + TextUtils.join(",", deleteIds) + ")";
                getContentResolver().delete(uri, where, null);
            }
            Log.d(getClass().getName(), deleteIds.size() + " feeds was completely deleted");
        }else {
            Log.d(getClass().getName(), "cursor null");
        }
    }
}
