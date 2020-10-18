package sk.ikim23.rsswatcher.data;

import android.content.ContentProvider;
import android.content.ContentUris;
import android.content.ContentValues;
import android.content.UriMatcher;
import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;
import android.net.Uri;
import android.support.annotation.NonNull;
import android.support.annotation.Nullable;
import android.util.Log;

import sk.ikim23.rsswatcher.U;

import static sk.ikim23.rsswatcher.data.DbHelper.CHANNEL_ID;
import static sk.ikim23.rsswatcher.data.DbHelper.CHANNEL_NAME;
import static sk.ikim23.rsswatcher.data.DbHelper.FEED_IS_DELETED;
import static sk.ikim23.rsswatcher.data.DbHelper.FEED_IS_READ;

public class FeedProvider extends ContentProvider {


    public static final String AUTHORITY = "sk.ikim23.rsswatcher.provider";
    public static final String PATH_CHANNEL_JOIN_FEED = "channelJoinFeed";
    public static final String PATH_CHANNEL = DbHelper.TABLE_CHANNEL;
    public static final String PATH_FEED = DbHelper.TABLE_FEED;
    public static final String PATH_FEED_NO_UPDATE = DbHelper.TABLE_FEED + "_no_update";
    public static final String PATH_MATCH = "query";
    public static final String PATH_FILTER = DbHelper.TABLE_FILTER;
    public static final String PATH_NOTIFICATION = DbHelper.TABLE_NOTIFICATION;

    private static final int CHANNEL_JOIN_COUNT = 1;
    private static final int CHANNEL_ALL = 2;
    private static final int CHANNEL_SINGLE = 3;
    private static final int FEED_ALL = 4;
    private static final int FEED_SINGLE = 5;
    private static final int MATCH = 6;
    private static final int FILTER = 7;
    private static final int NOTIFICATION = 8;
    private static final int FEED_NO_UPDATE = 9;
    private static final UriMatcher uriMatcher = new UriMatcher(UriMatcher.NO_MATCH);

    static {
        uriMatcher.addURI(AUTHORITY, PATH_CHANNEL_JOIN_FEED, CHANNEL_JOIN_COUNT);
        uriMatcher.addURI(AUTHORITY, PATH_CHANNEL, CHANNEL_ALL);
        uriMatcher.addURI(AUTHORITY, PATH_CHANNEL + "/#", CHANNEL_SINGLE);
        uriMatcher.addURI(AUTHORITY, PATH_FEED, FEED_ALL);
        uriMatcher.addURI(AUTHORITY, PATH_FEED + "/#", FEED_SINGLE);
        uriMatcher.addURI(AUTHORITY, PATH_MATCH, MATCH);
        uriMatcher.addURI(AUTHORITY, PATH_FILTER, FILTER);
        uriMatcher.addURI(AUTHORITY, PATH_NOTIFICATION, NOTIFICATION);
        uriMatcher.addURI(AUTHORITY, PATH_FEED_NO_UPDATE, FEED_NO_UPDATE);
    }

    public static final Uri URI_CHANNEL_JOIN_FEED = Uri.parse("content://" + AUTHORITY + "/" + PATH_CHANNEL_JOIN_FEED);

    private DbHelper helper;

    @Override
    public boolean onCreate() {
        helper = new DbHelper(getContext());
        return true;
    }

    @Nullable
    @Override
    public Cursor query(@NonNull Uri uri, String[] projection, String selection, String[] selectionArgs, String sortOrder) {
        SQLiteDatabase db = helper.getReadableDatabase();
        Cursor cursor = null;
        String rowId;
        switch (uriMatcher.match(uri)) {
            case CHANNEL_JOIN_COUNT:
                cursor = db.rawQuery(String.format("SELECT %s,ifnull(SUM(1 - f.%s),0) FROM %s c LEFT JOIN %s f ON(c.%s = f.%s)  WHERE ifnull(f.%s,0) = 0 GROUP BY c.%s ORDER BY c.%s;",
                        projection(projection, "c."), FEED_IS_READ,
                        DbHelper.TABLE_CHANNEL, DbHelper.TABLE_FEED,
                        DbHelper.CHANNEL_ID, DbHelper.FEED_CHANNEL_ID,
                        FEED_IS_DELETED,
                        CHANNEL_ID, CHANNEL_NAME
                ), null);
                break;
            case CHANNEL_ALL:
                cursor = db.query(DbHelper.TABLE_CHANNEL, projection, selection, null, null, null, sortOrder);
                break;
            case CHANNEL_SINGLE:
                rowId = uri.getPathSegments().get(1);
                cursor = db.query(DbHelper.TABLE_CHANNEL, projection, DbHelper.CHANNEL_ID + '=' + rowId, null, null, null, null);
                break;
            case FEED_ALL:
            case MATCH:
                cursor = db.query(DbHelper.TABLE_FEED, projection, selection, null, null, null, sortOrder);
                break;
            case FEED_NO_UPDATE:
                return db.query(DbHelper.TABLE_FEED, projection, selection, null, null, null, sortOrder);
            case FEED_SINGLE:
                rowId = uri.getPathSegments().get(1);
                cursor = db.query(DbHelper.TABLE_FEED, projection, DbHelper.FEED_ID + '=' + rowId, null, null, null, null);
                break;
            case FILTER:
                cursor = db.query(DbHelper.TABLE_FILTER, projection, selection, null, null, null, sortOrder);
                break;
            case NOTIFICATION:
                cursor = db.query(DbHelper.TABLE_NOTIFICATION, projection, selection, null, null, null, sortOrder);
                break;
        }
        if (cursor != null) {
            cursor.setNotificationUri(getContext().getContentResolver(), uri);
        }
        return cursor;
    }

    @Nullable
    @Override
    public Uri insert(@NonNull Uri uri, ContentValues values) {
        SQLiteDatabase db = helper.getWritableDatabase();
        long rowId = -1;
        Uri rowUri = null;
        switch (uriMatcher.match(uri)) {
            case CHANNEL_ALL:
                rowId = db.insertOrThrow(DbHelper.TABLE_CHANNEL, null, values);
//                getContext().getContentResolver().notifyChange(rowUri, null);
                getContext().getContentResolver().notifyChange(URI_CHANNEL_JOIN_FEED, null);
                break;
            case FEED_ALL:
                // URI_CHANNEL_JOIN_FEED is notified after insertion from FeedDownloadTask
                rowId = db.insertOrThrow(DbHelper.TABLE_FEED, null, values);
                if (rowId != -1) {
                    String title = values.getAsString(DbHelper.FEED_TITLE);
                    String description = values.getAsString(DbHelper.FEED_DESCRIPTION_NO_HTML);
                    db.execSQL(String.format("INSERT INTO %s(docid, %s, %s) VALUES(?, ?, ?)", DbHelper.TABLE_FEED_FTS, DbHelper.FEED_TITLE, DbHelper.FEED_DESCRIPTION),
                            new Object[]{rowId, title != null ? U.normalize(title) : "", description != null ? U.normalize(description) : ""});
                }
                break;
            case FILTER:
                rowId = db.insertOrThrow(DbHelper.TABLE_FILTER, null, values);
                break;
            case NOTIFICATION:
                rowId = db.insertOrThrow(DbHelper.TABLE_NOTIFICATION, null, values);
                break;
        }
        if (rowId != -1) {
            rowUri = ContentUris.withAppendedId(uri, rowId);
            getContext().getContentResolver().notifyChange(rowUri, null);
        }
        return rowUri;
    }

    @Override
    public int delete(@NonNull Uri uri, String selection, String[] selectionArgs) {
        SQLiteDatabase db = helper.getWritableDatabase();
        int deleteCount = 0;
        switch (uriMatcher.match(uri)) {
            case CHANNEL_SINGLE:
                String rowId = uri.getPathSegments().get(1);
                db.delete(DbHelper.TABLE_FILTER, DbHelper.FILTER_CHANNEL_ID + "=" + rowId, null);
                db.delete(DbHelper.TABLE_FEED, DbHelper.FEED_CHANNEL_ID + "=" + rowId, null);
                deleteCount = db.delete(DbHelper.TABLE_CHANNEL, DbHelper.CHANNEL_ID + "=" + rowId, null);
                getContext().getContentResolver().notifyChange(uri, null);
                getContext().getContentResolver().notifyChange(URI_CHANNEL_JOIN_FEED, null);
                break;
            case FEED_ALL:
                deleteCount = db.delete(DbHelper.TABLE_FEED, selection, null);
                String sql = "DELETE FROM " + DbHelper.TABLE_FEED_FTS;
                if (selection != null) {
                    selection = selection.replace(DbHelper.FEED_ID, "docid");
                    sql += " WHERE " + selection;
                }
                db.execSQL(sql);
                getContext().getContentResolver().notifyChange(uri, null);
                getContext().getContentResolver().notifyChange(URI_CHANNEL_JOIN_FEED, null);
                break;
            case FILTER:
                deleteCount = db.delete(DbHelper.TABLE_FILTER, selection, null);
                getContext().getContentResolver().notifyChange(uri, null);
                return deleteCount;
            case NOTIFICATION:
                deleteCount = db.delete(DbHelper.TABLE_NOTIFICATION, selection, null);
                getContext().getContentResolver().notifyChange(uri, null);
                return deleteCount;
        }
        return deleteCount;
    }

    @Override
    public int update(@NonNull Uri uri, ContentValues values, String selection, String[] selectionArgs) {
        SQLiteDatabase db = helper.getReadableDatabase();
        int updated = 0;
        String rowId;
        switch (uriMatcher.match(uri)) {
            case CHANNEL_SINGLE:
                rowId = uri.getPathSegments().get(1);
                updated = db.update(DbHelper.TABLE_CHANNEL, values, DbHelper.CHANNEL_ID + '=' + rowId, null);
                break;
            case FEED_SINGLE:
                rowId = uri.getPathSegments().get(1);
                Log.d("ContentProvider", "update feed " + rowId);
                updated = db.update(DbHelper.TABLE_FEED, values, DbHelper.FEED_ID + '=' + rowId, null);
                break;
            case FEED_ALL:
                updated = db.update(DbHelper.TABLE_FEED, values, selection, null);
                break;
            case FILTER:
                updated = db.update(DbHelper.TABLE_FILTER, values, selection, null);
                getContext().getContentResolver().notifyChange(uri, null);
                return updated;
        }
        if (updated > 0) {
            getContext().getContentResolver().notifyChange(uri, null);
            getContext().getContentResolver().notifyChange(URI_CHANNEL_JOIN_FEED, null);
        }
        return updated;
    }

    @Nullable
    @Override
    public String getType(@NonNull Uri uri) {
        return null;
    }

    private String projection(String[] projection, String prefix) {
        StringBuilder sb = new StringBuilder();
        for (String arg : projection) {
            sb.append(prefix).append(arg).append(',');
        }
        return sb.substring(0, sb.length() - 1);
    }

}
