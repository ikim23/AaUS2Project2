package sk.ikim23.rsswatcher.service;

import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.ContentValues;
import android.content.Context;
import android.content.Intent;
import android.database.Cursor;
import android.graphics.Color;
import android.media.RingtoneManager;
import android.net.Uri;
import android.os.AsyncTask;
import android.support.v4.app.NotificationCompat;
import android.support.v4.app.TaskStackBuilder;
import android.support.v4.content.LocalBroadcastManager;
import android.util.Log;

import com.google.code.rome.android.repackaged.com.sun.syndication.feed.synd.SyndEntry;
import com.google.code.rome.android.repackaged.com.sun.syndication.feed.synd.SyndFeed;
import com.google.code.rome.android.repackaged.com.sun.syndication.io.FeedException;
import com.google.code.rome.android.repackaged.com.sun.syndication.io.SyndFeedInput;
import com.google.code.rome.android.repackaged.com.sun.syndication.io.XmlReader;

import org.jsoup.Jsoup;

import java.io.IOException;
import java.net.URL;
import java.util.HashSet;

import sk.ikim23.rsswatcher.R;
import sk.ikim23.rsswatcher.U;
import sk.ikim23.rsswatcher.activity.feed.FeedActivity;
import sk.ikim23.rsswatcher.activity.notification.NotificationResultActivity;
import sk.ikim23.rsswatcher.data.DbHelper;
import sk.ikim23.rsswatcher.data.FeedProvider;

public class FeedDownloadTask extends AsyncTask<Void, Void, Void> {

    private static final long SEARCH_START_ID_DEFAULT = -1;
    private final Uri feedUri = Uri.parse("content://" + FeedProvider.AUTHORITY + "/" + FeedProvider.PATH_FEED);
    private final Intent resultIntent = new Intent(U.DOWNLOAD_FINISHED);
    private final Context context;
    private long channelId;

    public FeedDownloadTask(Context context, long channelId) {
        this.context = context;
        this.channelId = channelId;
    }

    @Override
    protected Void doInBackground(Void... voids) {
        Log.d(getClass().getName(), "Starting FeedDownloadTask");
        if (!U.isConnected(context)) {
            Log.d(getClass().getName(), "No internet connection");
            return null;
        }
        Uri channelsUri = Uri.parse("content://" + FeedProvider.AUTHORITY + "/" + FeedProvider.PATH_CHANNEL);
        String selection = channelId == U.CHANNEL_ID_UNDEFINED ? null : DbHelper.CHANNEL_ID + "=" + channelId;
        Cursor cursor = context.getContentResolver().query(channelsUri, new String[]{DbHelper.CHANNEL_ID, DbHelper.CHANNEL_NAME, DbHelper.CHANNEL_URL}, selection, null, null);
        if (cursor == null) {
            return null;
        }
        String channelName, channelUrl;
        ContentValues channelVals = new ContentValues();
        cursor.moveToFirst();
        while (!cursor.isAfterLast()) {
            channelId = cursor.getLong(cursor.getColumnIndex(DbHelper.CHANNEL_ID));
            channelName = cursor.getString(cursor.getColumnIndex(DbHelper.CHANNEL_NAME));
            channelUrl = cursor.getString(cursor.getColumnIndex(DbHelper.CHANNEL_URL));
            updateChannel(channelId, channelName, channelUrl);
            // set channel update time
            Uri channelUri = Uri.parse("content://" + FeedProvider.AUTHORITY + "/" + FeedProvider.PATH_CHANNEL + "/" + channelId);
            channelVals.put(DbHelper.CHANNEL_LAST_UPDATE, System.currentTimeMillis());
            context.getContentResolver().update(channelUri, channelVals, null, null);
            cursor.moveToNext();
        }
        cursor.close();

        // broadcast callback triggers DrawerActivity.loadCurrentChannel() -> this reloads navigation drawer counters of unread feeds
        context.getContentResolver().notifyChange(FeedProvider.URI_CHANNEL_JOIN_FEED, null);
        LocalBroadcastManager.getInstance(context).sendBroadcast(resultIntent);
        Log.d(getClass().getName(), "Ending FeedDownloadTask");
        return null;
    }

    private void updateChannel(long channelId, String channelName, String channelUrl) {
        Log.d(getClass().getName(), "Downloading from: " + channelUrl);
        try {
            SyndFeed syndFeed = new SyndFeedInput().build(new XmlReader(new URL(channelUrl)));
            // load urls of feeds
            HashSet<String> guids = new HashSet<>();
            Cursor cursor = context.getContentResolver().query(feedUri, new String[]{DbHelper.FEED_GUID}, DbHelper.FEED_CHANNEL_ID + "=" + channelId, null, null);
            if (cursor == null) {
                return;
            }
            cursor.moveToFirst();
            while (!cursor.isAfterLast()) {
                guids.add(cursor.getString(cursor.getColumnIndex(DbHelper.FEED_GUID)));
                cursor.moveToNext();
            }
            cursor.close();
            Uri resultUri;
            long searchStartId = SEARCH_START_ID_DEFAULT;
            ContentValues values = new ContentValues();
            SyndEntry syndEntry;
            int count = 0;
            for (Object o : syndFeed.getEntries()) {
                syndEntry = (SyndEntry) o;
                if (!guids.contains(syndEntry.getUri())) {
                    values.clear();
                    values.put(DbHelper.FEED_CHANNEL_ID, channelId);
                    values.put(DbHelper.FEED_GUID, syndEntry.getUri());
                    values.put(DbHelper.FEED_TITLE, syndEntry.getTitle());
                    long pubDate = syndEntry.getPublishedDate() != null ? syndEntry.getPublishedDate().getTime() : System.currentTimeMillis();
                    values.put(DbHelper.FEED_PUB_DATE, pubDate);
                    if (U.isValidURL(syndEntry.getLink())) {
                        values.put(DbHelper.FEED_LINK, syndEntry.getLink());
                    }
                    if (syndEntry.getDescription() != null) {
                        String description = syndEntry.getDescription().getValue();
                        values.put(DbHelper.FEED_DESCRIPTION, description);
                        String noHtml = Jsoup.parse(description).text();
                        values.put(DbHelper.FEED_DESCRIPTION_NO_HTML, noHtml);
                    }
                    resultUri = context.getContentResolver().insert(feedUri, values);
                    if (searchStartId == SEARCH_START_ID_DEFAULT && resultUri != null) {
                        searchStartId = Long.parseLong(resultUri.getPathSegments().get(1));
                    }
                    count++;
                }
            }
            Log.d(getClass().getName(), count + " new feeds downloaded");
            search(channelId, channelName, searchStartId);
        } catch (IOException e) {
            Log.e(getClass().getName(), "Reading stream from URL: " + channelUrl, e);
            e.printStackTrace();
            resultIntent.putExtra(U.EXTRA_ERROR, R.string.error_message_downloading);
        } catch (FeedException e) {
            Log.e(getClass().getName(), "Feed could not be parsed.", e);
            e.printStackTrace();
            resultIntent.putExtra(U.EXTRA_ERROR, R.string.error_message_parsing);
        }
    }

    private void search(long channelId, String channelName, long searchStartId) {
        if (searchStartId != SEARCH_START_ID_DEFAULT) {
            Uri filterUri = Uri.parse("content://" + FeedProvider.AUTHORITY + "/" + FeedProvider.PATH_FILTER);
            String where = DbHelper.FILTER_CHANNEL_ID + " = " + channelId;
            Cursor filterCursor = context.getContentResolver().query(filterUri, new String[]{DbHelper.FILTER_ID, DbHelper.FILTER_MATCH_QUERY}, where, null, null);
            if (filterCursor != null && filterCursor.moveToFirst()) {
                long filterId;
                String query, matchQuery;
                where = String.format("%s = %d AND %s >= %d AND %s IN (SELECT docid FROM %s WHERE %s MATCH '%%s')",
                        DbHelper.FEED_CHANNEL_ID, channelId,
                        DbHelper.FEED_ID, searchStartId,
                        DbHelper.FEED_ID,
                        DbHelper.TABLE_FEED_FTS,
                        DbHelper.TABLE_FEED_FTS);
                Uri feedUri = Uri.parse("content://" + FeedProvider.AUTHORITY + "/" + FeedProvider.PATH_FEED);
                while (!filterCursor.isAfterLast()) {
                    filterId = filterCursor.getLong(filterCursor.getColumnIndex(DbHelper.FILTER_ID));
                    query = filterCursor.getString(filterCursor.getColumnIndex(DbHelper.FILTER_MATCH_QUERY));
                    matchQuery = U.normalize(query);
                    Log.d(getClass().getName(), "searching for channel: " + channelName + " filter: " + matchQuery);
                    Cursor c = context.getContentResolver().query(feedUri, new String[]{DbHelper.FEED_ID}, String.format(where, matchQuery), null, null);
                    if (c != null && c.moveToFirst()) {
                        Uri notificationUri = Uri.parse("content://" + FeedProvider.AUTHORITY + "/" + FeedProvider.PATH_NOTIFICATION);
                        ContentValues values = new ContentValues();
                        values.put(DbHelper.NOTIFICATION_FILTER_ID, filterId);
                        while (!c.isAfterLast()) {
                            values.put(DbHelper.NOTIFICATION_FEED_ID, c.getLong(c.getColumnIndex(DbHelper.FEED_ID)));
                            Log.d(getClass().getName(), values.toString());
                            context.getContentResolver().insert(notificationUri, values);
                            c.moveToNext();
                        }
                        if (c.getCount() > 0) {
                            notification(filterId, query, channelName);
                        }
                        c.close();
                    }
                    filterCursor.moveToNext();
                }
                filterCursor.close();
            }
        }
    }

    private void notification(long filterId, String filter, String channelName) {
        Uri notificationUri = Uri.parse("content://" + FeedProvider.AUTHORITY + "/" + FeedProvider.PATH_NOTIFICATION);
        String where = DbHelper.NOTIFICATION_FILTER_ID + "=" + filterId;
        Cursor c = context.getContentResolver().query(notificationUri, new String[]{DbHelper.NOTIFICATION_FEED_ID}, where, null, null);
        if (c != null && c.moveToFirst()) {
            StringBuilder feedWhere = new StringBuilder();
            feedWhere.append(DbHelper.FEED_ID).append(" IN (");
            while (!c.isAfterLast()) {
                feedWhere.append(c.getLong(c.getColumnIndex(DbHelper.NOTIFICATION_FEED_ID))).append(',');
                c.moveToNext();
            }
            feedWhere.setCharAt(feedWhere.length() - 1, ')');

            Log.d(getClass().getName(), "notification: " + feedWhere.toString());
            int feedCount = c.getCount();
            if (feedCount > 0) {
                final Class resultClass = feedCount == 1 ? FeedActivity.class : NotificationResultActivity.class;
                Intent resultIntent = new Intent(context, resultClass);
                resultIntent.putExtra(U.EXTRA_CHANNEL_NAME, channelName);
                resultIntent.putExtra(U.EXTRA_URI, "content://" + FeedProvider.AUTHORITY + "/" + FeedProvider.PATH_FEED);
                resultIntent.putExtra(U.EXTRA_WHERE_CLAUSE, feedWhere.toString());
                resultIntent.putExtra(U.EXTRA_POSITION, 0);
                resultIntent.putExtra(U.EXTRA_FILTER_ID, filterId);

                Intent deleteIntent = new Intent(context, NotificationDeleteService.class);
                deleteIntent.putExtra(U.EXTRA_FILTER_ID, filterId);

                TaskStackBuilder stackBuilder = TaskStackBuilder.create(context);
                stackBuilder.addParentStack(resultClass);
                stackBuilder.addNextIntent(resultIntent);

                int id = U.longToInt(filterId);

                NotificationCompat.Builder builder = new NotificationCompat.Builder(context)
                        .setSmallIcon(R.drawable.ic_rss_feed_black_24dp)
                        .setContentTitle(channelName)
                        .setContentText(feedCount + " matches for filter: " + filter)
                        .setContentIntent(stackBuilder.getPendingIntent(id, PendingIntent.FLAG_UPDATE_CURRENT))
                        // called only on swipe or clear all, must call explicitly if user click notification
                        .setDeleteIntent(PendingIntent.getService(context, id, deleteIntent, PendingIntent.FLAG_UPDATE_CURRENT))
                        .setAutoCancel(true)
                        .setVisibility(NotificationCompat.VISIBILITY_PUBLIC)
                        .setSound(RingtoneManager.getDefaultUri(RingtoneManager.TYPE_NOTIFICATION))
                        .setVibrate(new long[]{500, 500, 500, 500})
                        .setLights(Color.GREEN, 500, 500);

                NotificationManager manager = (NotificationManager) context.getSystemService(Context.NOTIFICATION_SERVICE);
                manager.notify(id, builder.build());
            }
            c.close();
        }
    }
}
