package sk.ikim23.rsswatcher.data;

import android.content.Context;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteOpenHelper;
import android.util.Log;

public class DbHelper extends SQLiteOpenHelper {

    public static final String TABLE_CHANNEL = "channel";
    public static final String CHANNEL_ID = "_id";
    public static final String CHANNEL_NAME = "name";
    public static final String CHANNEL_URL = "url";
    public static final String CHANNEL_ICON_PATH = "icon_path";
    public static final String CHANNEL_LAST_UPDATE = "last_update";

    public static final String TABLE_FEED_FTS = "feed_fts";
    public static final String TABLE_FEED = "feed";
    public static final String FEED_ID = "_id";
    public static final String FEED_CHANNEL_ID = "channel_id";
    public static final String FEED_GUID = "guid";
    public static final String FEED_LINK = "link";
    public static final String FEED_TITLE = "title";
    public static final String FEED_DESCRIPTION = "description";
    public static final String FEED_DESCRIPTION_NO_HTML = "description_no_html";
    public static final String FEED_PUB_DATE = "pub_date";
    public static final String FEED_IS_READ = "is_read";
    public static final String FEED_IS_ARCHIVED = "is_archived";
    public static final String FEED_IS_DELETED = "is_deleted";

    public static final String TABLE_FILTER = "filter";
    public static final String FILTER_ID = "_id";
    public static final String FILTER_CHANNEL_ID = "channel_id";
    public static final String FILTER_MATCH_QUERY = "query";

    public static final String TABLE_NOTIFICATION = "notification";
    public static final String NOTIFICATION_FILTER_ID = "filter_id";
    public static final String NOTIFICATION_FEED_ID = "feed_id";

    private static final String DATABASE_NAME = "feeds.db";
    private static final int DATABASE_VERSION = 1;

    public DbHelper(Context context) {
        super(context, DATABASE_NAME, null, DATABASE_VERSION);
    }

    @Override
    public void onCreate(SQLiteDatabase db) {
        db.execSQL(String.format("CREATE TABLE %s(%s INTEGER PRIMARY KEY AUTOINCREMENT, %s TEXT NOT NULL, %s TEXT NOT NULL, %s TEXT, %s INTEGER)",
                TABLE_CHANNEL,
                CHANNEL_ID, CHANNEL_NAME, CHANNEL_URL, CHANNEL_ICON_PATH, CHANNEL_LAST_UPDATE));
        db.execSQL(String.format("CREATE TABLE %s(%s INTEGER PRIMARY KEY AUTOINCREMENT, %s INTEGER NOT NULL, %s TEXT NOT NULL, %s TEXT, %s TEXT NOT NULL, %s TEXT, %s TEXT, %s INTEGER NOT NULL, %s INTEGER NOT NULL DEFAULT 0, %s INTEGER NOT NULL DEFAULT 0, %s INTEGER NOT NULL DEFAULT 0, FOREIGN KEY(%s) REFERENCES %s(%s))",
                TABLE_FEED,
                FEED_ID, FEED_CHANNEL_ID, FEED_GUID, FEED_LINK, FEED_TITLE, FEED_DESCRIPTION, FEED_DESCRIPTION_NO_HTML, FEED_PUB_DATE, FEED_IS_READ, FEED_IS_ARCHIVED, FEED_IS_DELETED,
                FEED_CHANNEL_ID, TABLE_CHANNEL, CHANNEL_ID));
        db.execSQL(String.format("CREATE VIRTUAL TABLE %s USING fts4(content=\"%s\", %s, %s)",
                TABLE_FEED_FTS,
                TABLE_FEED, FEED_TITLE, FEED_DESCRIPTION));
        db.execSQL(String.format("CREATE TABLE %s(%s INTEGER PRIMARY KEY AUTOINCREMENT, %s INTEGER NOT NULL, %s TEXT NOT NULL, FOREIGN KEY(%s) REFERENCES %s(%s))",
                TABLE_FILTER,
                FILTER_ID, FILTER_CHANNEL_ID, FILTER_MATCH_QUERY,
                FILTER_CHANNEL_ID, TABLE_CHANNEL, CHANNEL_ID));
        db.execSQL(String.format("CREATE TABLE %s (%s INTEGER NOT NULL, %s INTEGER NOT NULL, FOREIGN KEY(%s) REFERENCES %s(%s), FOREIGN KEY(%s) REFERENCES %s(%s), PRIMARY KEY (%s, %s))",
                TABLE_NOTIFICATION,
                NOTIFICATION_FILTER_ID, NOTIFICATION_FEED_ID,
                NOTIFICATION_FILTER_ID, TABLE_FILTER, FILTER_ID,
                NOTIFICATION_FEED_ID, TABLE_FEED, FEED_ID,
                NOTIFICATION_FILTER_ID, NOTIFICATION_FEED_ID));
    }

    @Override
    public void onUpgrade(SQLiteDatabase db, int oldVersion, int newVersion) {
        Log.i(DbHelper.class.getName(), String.format("Upgrading database from version: %d to: %d.", oldVersion, newVersion));
        db.execSQL("DROP TABLE IF EXISTS " + TABLE_NOTIFICATION);
        db.execSQL("DROP TABLE IF EXISTS " + TABLE_FILTER);
        db.execSQL("DROP TABLE IF EXISTS " + TABLE_FEED_FTS);
        db.execSQL("DROP TABLE IF EXISTS " + TABLE_FEED);
        db.execSQL("DROP TABLE IF EXISTS " + TABLE_CHANNEL);
        onCreate(db);
    }

}
