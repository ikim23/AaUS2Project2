package sk.ikim23.rsswatcher.activity.feed;

import android.content.ContentValues;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.IntentFilter;
import android.database.Cursor;
import android.graphics.Point;
import android.net.ConnectivityManager;
import android.net.Uri;
import android.os.Bundle;
import android.support.design.widget.AppBarLayout;
import android.support.design.widget.FloatingActionButton;
import android.support.v4.view.ViewPager;
import android.support.v7.app.ActionBar;
import android.support.v7.app.AlertDialog;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;
import android.util.Log;
import android.view.Display;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.WindowManager;
import android.widget.ProgressBar;
import android.widget.RelativeLayout;
import android.widget.TextView;

import com.google.android.gms.ads.AdListener;
import com.google.android.gms.ads.AdRequest;
import com.google.android.gms.ads.AdView;
import com.google.android.gms.ads.MobileAds;

import sk.ikim23.rsswatcher.R;
import sk.ikim23.rsswatcher.U;
import sk.ikim23.rsswatcher.activity.ConnectivityReceiver;
import sk.ikim23.rsswatcher.data.DbHelper;
import sk.ikim23.rsswatcher.data.FeedProvider;
import sk.ikim23.rsswatcher.service.NotificationDeleteService;

public class FeedActivity extends AppCompatActivity {

    static void showBadLinkAlert(Context context) {
        AlertDialog.Builder builder = new AlertDialog.Builder(context);
        builder.setTitle(R.string.dialog_bad_link_title);
        builder.setMessage(R.string.dialog_bad_link_message);
        builder.setPositiveButton(R.string.OK, new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                dialog.dismiss();
            }
        });
        AlertDialog dialog = builder.create();
        dialog.show();
    }

    private static final long ANIM_DURATION = 150L;
    private static final int LOADER_ID = 777;
    private AppBarLayout appBarLayout;
    private MenuItem archiveMenuItem;
    private TextView networkStatus;
    private ViewPager viewPager;
    private FeedPagerAdapter adapter;
    private FeedPagerAdapter.FeedPagerAdapterLoaderCallbacks loaderCallbacks;
    private ConnectivityReceiver connectivityReceiver;
    private int feedPosition;
    private ProgressBar progressBar;
    private FloatingActionButton floatingActionButton;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_feed);
        feedPosition = getIntent().getIntExtra(U.EXTRA_POSITION, 0);

        appBarLayout = (AppBarLayout) findViewById(R.id.app_bar_layout);
        networkStatus = (TextView) findViewById(R.id.network_status);
        progressBar = (ProgressBar) findViewById(R.id.progress_bar);
        viewPager = (ViewPager) findViewById(R.id.view_pager);
        floatingActionButton = (FloatingActionButton) findViewById(R.id.floating_button);

        initAdMob();
        initToolbar();
        initViewPager();
        initFloatingActionButton();

        Intent intent = getIntent();
        boolean isServiceExecuted = intent.getBooleanExtra(U.EXTRA_SERVICE_EXECUTED, false);
        if (!isServiceExecuted) {
            long filterId = intent.getLongExtra(U.EXTRA_FILTER_ID, U.FILTER_ID_DEFAULT);
            if (filterId != U.FILTER_ID_DEFAULT) {
                Intent serviceIntent = new Intent(this, NotificationDeleteService.class);
                serviceIntent.putExtra(U.EXTRA_FILTER_ID, filterId);
                startService(serviceIntent);
            }
            intent.putExtra(U.EXTRA_SERVICE_EXECUTED, true);
        }
    }

    @Override
    protected void onStart() {
        super.onStart();
        connectivityReceiver = new ConnectivityReceiver(networkStatus);
        registerReceiver(connectivityReceiver, new IntentFilter(ConnectivityManager.CONNECTIVITY_ACTION));
    }

    @Override
    protected void onStop() {
        super.onStop();
        unregisterReceiver(connectivityReceiver);
    }

    private void setToolbarSubtitle(int position) {
        feedPosition = position;
        int count = adapter.getCount();
        if (count > 0) {
            ActionBar actionBar = getSupportActionBar();
            if (actionBar != null) {
                actionBar.setSubtitle((feedPosition + 1) + "/" + count);
            }
        }
        Cursor cursor = adapter.getCursor();
        if (cursor != null && cursor.moveToPosition(feedPosition)) {
            long feedId = cursor.getLong(cursor.getColumnIndex(DbHelper.FEED_ID));
            // set correct floating button icon
            floatingActionButton.setImageResource(!FeedFragment.loadedWeb.contains(feedId) ? R.drawable.ic_language_white_24dp : R.drawable.ic_arrow_back_white_24dp);
            // hide progress bar on new page
            if (progressBar.getVisibility() == View.VISIBLE) {
                progressBar.setVisibility(View.GONE);
                progressBar.setProgress(0);
            }
            // scroll down toolbar
            appBarLayout.setExpanded(true, true);
            if (cursor.getInt(cursor.getColumnIndex(DbHelper.FEED_IS_READ)) == 0) {
                ContentValues values = new ContentValues();
                values.put(DbHelper.FEED_IS_READ, 1);
                Uri uri = Uri.parse("content://" + FeedProvider.AUTHORITY + "/" + FeedProvider.PATH_FEED + "/" + feedId);
                getContentResolver().update(uri, values, null, null);
            }
        }
    }

    // ****************************************
    // OPTIONS MENU
    // ****************************************

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.activity_feed_menu, menu);
        archiveMenuItem = menu.findItem(R.id.action_archive);
        Log.d(getClass().getName(), "archive item set");
        Cursor cursor = adapter.getCursor();
        if (cursor != null && cursor.moveToPosition(feedPosition)) {
            long feedId = cursor.getLong(cursor.getColumnIndex(DbHelper.FEED_ID));
            boolean isArchived = isArchived(feedId); // cursor.getInt(cursor.getColumnIndex(DbHelper.FEED_IS_ARCHIVED)) != 0;
            setArchiveMenuItem(isArchived);
        }
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        Cursor cursor;
        switch (item.getItemId()) {
            case android.R.id.home:
                onBackPressed();
                return true;
            case R.id.action_share:
                cursor = adapter.getCursor();
                if (cursor != null && cursor.moveToPosition(feedPosition)) {
                    String feedLink = cursor.getString(cursor.getColumnIndex(DbHelper.FEED_LINK));
                    if (feedLink != null) {
                        Intent sendIntent = new Intent();
                        sendIntent.setAction(Intent.ACTION_SEND);
                        sendIntent.putExtra(Intent.EXTRA_TEXT, feedLink);
                        sendIntent.setType("text/plain");
                        startActivity(Intent.createChooser(sendIntent, getString(R.string.share)));
                    } else {
                        showBadLinkAlert(this);
                    }
                }
                return true;
            case R.id.action_archive:
                cursor = adapter.getCursor();
                if (cursor != null && cursor.moveToPosition(feedPosition)) {
                    long feedId = cursor.getLong(cursor.getColumnIndex(DbHelper.FEED_ID));
                    boolean isArchived = isArchived(feedId); // cursor.getInt(cursor.getColumnIndex(DbHelper.FEED_IS_ARCHIVED)) != 0;
                    Uri uri = Uri.parse("content://" + FeedProvider.AUTHORITY + "/" + FeedProvider.PATH_FEED + "/" + feedId);
                    ContentValues values = new ContentValues();
                    values.put(DbHelper.FEED_IS_ARCHIVED, isArchived ? 0 : 1);
                    getContentResolver().update(uri, values, null, null);
                    setArchiveMenuItem(!isArchived);
                    //
                }
                return true;
            default:
                return super.onOptionsItemSelected(item);
        }
    }

    private boolean isArchived(long feedId) {
        Uri uri = Uri.parse("content://" + FeedProvider.AUTHORITY + "/" + FeedProvider.PATH_FEED);
        String where = DbHelper.FEED_ID + "=" + feedId;
        Cursor c = getContentResolver().query(uri, new String[]{DbHelper.FEED_IS_ARCHIVED}, where, null, null);
        return c != null && c.moveToFirst() && c.getInt(c.getColumnIndex(DbHelper.FEED_IS_ARCHIVED)) != 0;
    }

    private void setArchiveMenuItem(boolean isArchived) {
        if (archiveMenuItem != null) {
            if (isArchived) {
                archiveMenuItem.setIcon(R.drawable.ic_star_white_24dp);
                archiveMenuItem.setTitle(R.string.unarchive);
            } else {
                archiveMenuItem.setIcon(R.drawable.ic_star_border_white_24dp);
                archiveMenuItem.setTitle(R.string.archive);
            }
        } else {
            Log.d(getClass().getName(), "archive menu item is null on init");
        }
    }

    // ****************************************
    // SETTERS FOR FeedFragment
    // ****************************************

    void setBarProgressVisibility(int visibility) {
        progressBar.setVisibility(visibility);
    }

    void setBarProgress(int progress) {
        progressBar.setProgress(progress);
    }

    // moves to clicked feed
    void setCurrentItem(Cursor cursor) {
        setToolbarSubtitle(feedPosition);
        viewPager.setCurrentItem(feedPosition, false);
    }

    // ****************************************
    // SETUP METHODS
    // ****************************************

    private void initFloatingActionButton() {
        floatingActionButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                FeedFragment fragment = adapter.getCurrentFragment();
                if (fragment.isFeedDisplayed()) {
                    if (U.isConnected(FeedActivity.this)) {
                        if (fragment.showWeb()) {
                            floatingActionButton.setImageResource(R.drawable.ic_arrow_back_white_24dp);
                        }
                    } else {
                        connectivityReceiver.checkConnectivity(FeedActivity.this);
                    }
                } else {
                    fragment.showFeed();
                    floatingActionButton.setImageResource(R.drawable.ic_language_white_24dp);
                }
            }
        });
    }

    private void initViewPager() {
        adapter = new FeedPagerAdapter(getSupportFragmentManager());
        viewPager.setAdapter(adapter);
        viewPager.addOnPageChangeListener(new ViewPager.SimpleOnPageChangeListener() {
            @Override
            public void onPageSelected(int position) {
                setToolbarSubtitle(position);
                Cursor cursor = adapter.getCursor();
                if (cursor != null && cursor.moveToPosition(position)) {
                    long feedId = cursor.getLong(cursor.getColumnIndex(DbHelper.FEED_ID));
                    boolean isArchived = isArchived(feedId); // cursor.getInt(cursor.getColumnIndex(DbHelper.FEED_IS_ARCHIVED)) != 0;
                    setArchiveMenuItem(isArchived);
                }
            }

            @Override
            public void onPageScrollStateChanged(int state) {
                switch (state) {
                    case ViewPager.SCROLL_STATE_DRAGGING:
                        int position = viewPager.getCurrentItem();
                        if (0 < position && position < adapter.getCount() - 1) {
                            float transition = getWindowHeight() - floatingActionButton.getY();
                            floatingActionButton.animate().setDuration(ANIM_DURATION).translationY(transition);
                            break;
                        }
                    case ViewPager.SCROLL_STATE_SETTLING:
                        floatingActionButton.animate().setStartDelay(ANIM_DURATION / 2).setDuration(ANIM_DURATION).translationY(0);
                        break;
                }
                super.onPageScrollStateChanged(state);
            }
        });
        loaderCallbacks = new FeedPagerAdapter.FeedPagerAdapterLoaderCallbacks(this, adapter);
        getSupportLoaderManager().initLoader(LOADER_ID, getIntent().getExtras(), loaderCallbacks);
    }

    private void initToolbar() {
        Toolbar toolbar = (Toolbar) findViewById(R.id.toolbar);
        String activityTitle = getIntent().getStringExtra(U.EXTRA_CHANNEL_NAME);
        toolbar.setTitle(activityTitle);
        setSupportActionBar(toolbar);
        ActionBar actionBar = getSupportActionBar();
        if (actionBar != null) {
            getSupportActionBar().setDisplayHomeAsUpEnabled(true);
        }
    }

    private void initAdMob() {
        MobileAds.initialize(getApplicationContext(), getString(R.string.ad_app_id));
        final AdView adView = (AdView) findViewById(R.id.adView);
        adView.setAdListener(new AdListener() {
            @Override
            public void onAdLoaded() {
                super.onAdLoaded();
                View tempAd = findViewById(R.id.loading_ads);
                tempAd.setVisibility(View.GONE);
                RelativeLayout content = (RelativeLayout) findViewById(R.id.content);
                content.setPadding(0, 0, 0, adView.getHeight());
            }
        });
        AdRequest adRequest = new AdRequest.Builder().build();
        adView.loadAd(adRequest);
    }

    private int getWindowHeight() {
        WindowManager wm = (WindowManager) getSystemService(Context.WINDOW_SERVICE);
        Display display = wm.getDefaultDisplay();
        Point size = new Point();
        display.getSize(size);
        return size.y;
    }

}
