package sk.ikim23.rsswatcher.activity.drawer;

import android.content.BroadcastReceiver;
import android.content.ContentValues;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.IntentFilter;
import android.net.ConnectivityManager;
import android.net.Uri;
import android.os.Bundle;
import android.support.annotation.NonNull;
import android.support.design.widget.NavigationView;
import android.support.design.widget.TabLayout;
import android.support.v4.content.ContextCompat;
import android.support.v4.content.LocalBroadcastManager;
import android.support.v4.view.GravityCompat;
import android.support.v4.view.MenuItemCompat;
import android.support.v4.view.ViewPager;
import android.support.v4.widget.DrawerLayout;
import android.support.v4.widget.SwipeRefreshLayout;
import android.support.v7.app.ActionBarDrawerToggle;
import android.support.v7.app.AlertDialog;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.SearchView;
import android.support.v7.widget.Toolbar;
import android.text.format.DateUtils;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import java.util.Date;
import java.util.List;

import sk.ikim23.rsswatcher.R;
import sk.ikim23.rsswatcher.U;
import sk.ikim23.rsswatcher.activity.filter.FilterActivity;
import sk.ikim23.rsswatcher.activity.settings.SettingsActivity;
import sk.ikim23.rsswatcher.data.DbHelper;
import sk.ikim23.rsswatcher.data.FeedProvider;
import sk.ikim23.rsswatcher.service.DownloadService;
import sk.ikim23.rsswatcher.service.DeleteService;
import sk.ikim23.rsswatcher.view.MySwipeRefreshLayout;
import sk.ikim23.rsswatcher.view.MyViewPager;

public class DrawerActivity extends AppCompatActivity implements CurrentChannelLoader.OnCurrentChannelLoadListener, AddChannelDialog.OnChannelAddListener, NavigationView.OnNavigationItemSelectedListener, SwipeRefreshLayout.OnRefreshListener {

    private static final int MENU_LOADER_ID = 111;
    private DrawerLayout drawerLayout;
    private NavigationView navigationView;
    private Toolbar toolbar;
    private TabLayout tabLayout;
    private TextView networkStatus;
    private MySwipeRefreshLayout swipeRefreshLayout;
    private MyViewPager tabPager;
    private TabPagerAdapter tabAdapter;
    private Button btnAddChannel;
    private MenuItem searchMenuItem;
    private ChannelLoaderCallbacks menuLoader;
    private ConnectivityRefreshReceiver connectivityReceiver;
    private BroadcastReceiver downloadReceiver;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_drawer);
        drawerLayout = (DrawerLayout) findViewById(R.id.drawer_layout);
        navigationView = (NavigationView) findViewById(R.id.nav_view);
        toolbar = (Toolbar) findViewById(R.id.toolbar);
        tabLayout = (TabLayout) findViewById(R.id.tab_layout);
        networkStatus = (TextView) findViewById(R.id.network_status);
        swipeRefreshLayout = (MySwipeRefreshLayout) findViewById(R.id.swipe_refresh_layout);
        tabPager = (MyViewPager) findViewById(R.id.tab_pager);
        btnAddChannel = (Button) findViewById(R.id.btn_add_channel);
        initToolbar();
        initDrawerLayout();
        initTabLayout();
        initSwipeRefreshLayout();
        initBtnAddChannel();

        connectivityReceiver = new ConnectivityRefreshReceiver(networkStatus, swipeRefreshLayout);
        registerReceiver(connectivityReceiver, new IntentFilter(ConnectivityManager.CONNECTIVITY_ACTION));
        connectivityReceiver.checkConnectivity(this);

        CurrentChannelLoader.getInstance().addListener(this);
        CurrentChannelLoader.loadCurrentChannel(this);

        if (U.isConnected(this)) {
            swipeRefreshLayout.setRefreshing(true);
            downloadChannelsData();
        }
        // clear old data
        startService(new Intent(this, DeleteService.class));
    }

    @Override
    protected void onStart() {
        super.onStart();
        downloadReceiver = new BroadcastReceiver() {
            @Override
            public void onReceive(Context context, Intent intent) {
                Log.d(getClass().getName(), "Service broadcast received");
                if (swipeRefreshLayout.isRefreshing()) {
                    swipeRefreshLayout.setRefreshing(false);
                }
                int errorMsgId = intent.getIntExtra(U.EXTRA_ERROR, -1);
                if (errorMsgId != -1) {
                    connectivityReceiver.showErrorMessage(DrawerActivity.this, errorMsgId);
                }
                CurrentChannelLoader.loadCurrentChannel(DrawerActivity.this);
            }
        };
        LocalBroadcastManager.getInstance(this).registerReceiver(downloadReceiver, new IntentFilter(U.DOWNLOAD_FINISHED));
        connectivityReceiver.setEnabled(true);
    }

    @Override
    protected void onStop() {
        super.onStop();
        LocalBroadcastManager.getInstance(this).unregisterReceiver(downloadReceiver);
        connectivityReceiver.setEnabled(false);
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        unregisterReceiver(connectivityReceiver);
    }

    @Override
    public void onBackPressed() {
        if (drawerLayout.isDrawerOpen(GravityCompat.START)) {
            drawerLayout.closeDrawer(GravityCompat.START);
        } else {
            super.onBackPressed();
        }
    }

    @Override
    public void onChannelAdd(String channelName, String channelUrl) {
        Log.d(getClass().getName(), "Adding new Channel");
        ContentValues values = new ContentValues();
        values.put(DbHelper.CHANNEL_NAME, channelName);
        values.put(DbHelper.CHANNEL_URL, channelUrl);
        Uri uri = Uri.parse("content://" + FeedProvider.AUTHORITY + "/" + FeedProvider.PATH_CHANNEL);
        uri = getContentResolver().insert(uri, values);
        if (uri != null) {
            List<String> segments = uri.getPathSegments();
            if (segments != null) {
                try {
                    long newChannelId = Long.parseLong(segments.get(1));
                    CurrentChannelLoader.getInstance().setCurrentChannelId(newChannelId);

                    if (U.isConnected(this)) {
                        swipeRefreshLayout.setVisibility(View.VISIBLE);
                        swipeRefreshLayout.setRefreshing(true);
                    }
                    downloadChannelData(newChannelId);
                } catch (NumberFormatException e) {
                    e.printStackTrace();
                }
            }
        }
        CurrentChannelLoader.loadCurrentChannel(this);
    }

    // ****************************************
    // Disable views, search query
    // ****************************************

    void setViewsEnabled(boolean enabled) {
        // turn of all view that restricts bulk or other operation
        // NavigationDrawer, SwipeRefreshLayout, ViewPager, TabLayout
        tabLayout.setVisibility(enabled ? View.VISIBLE : View.GONE);
        swipeRefreshLayout.setEnabled(enabled && U.isConnected(this));
        tabPager.setPagingEnabled(enabled);
        drawerLayout.setDrawerLockMode(enabled ? DrawerLayout.LOCK_MODE_UNLOCKED : DrawerLayout.LOCK_MODE_LOCKED_CLOSED);
    }

    void setBulkActionEnabled(boolean enabled) {
        TabFragment tabFragment = tabAdapter.getCurrentFragment();
        tabFragment.setBulkActionEnabled(enabled);
    }

    void setMatchQuery(String matchQuery) {
        TabFragment tabFragment = tabAdapter.getCurrentFragment();
        tabFragment.setMatchQuery(matchQuery);
    }

    // ****************************************
    // OPTIONS MENU
    // ****************************************

    @Override
    public boolean onCreateOptionsMenu(final Menu menu) {
        getMenuInflater().inflate(R.menu.activity_drawer_menu, menu);
        searchMenuItem = menu.findItem(R.id.action_search);
        OnSearchActionMenu onSearchActionMenu = new OnSearchActionMenu(this, menu, searchMenuItem);
        MenuItemCompat.setOnActionExpandListener(searchMenuItem, onSearchActionMenu);
        SearchView searchView = (SearchView) searchMenuItem.getActionView();
        searchView.setOnQueryTextListener(onSearchActionMenu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        switch (item.getItemId()) {
            case R.id.action_search:
                return true;
            case R.id.action_delete_channel:
                final CurrentChannelLoader channel = CurrentChannelLoader.getInstance();
                if (channel.getCurrentChannelId() != U.CHANNEL_ID_UNDEFINED) {
                    AlertDialog.Builder builder = new AlertDialog.Builder(this);
                    builder.setTitle(channel.getCurrentChannelName());
                    builder.setMessage(R.string.dialog_delete_channel_message);
                    builder.setNegativeButton(R.string.cancel, new DialogInterface.OnClickListener() {
                        @Override
                        public void onClick(DialogInterface dialog, int which) {
                            dialog.cancel();
                        }
                    });
                    builder.setPositiveButton(R.string.OK, new DialogInterface.OnClickListener() {
                        @Override
                        public void onClick(DialogInterface dialog, int which) {
                            Uri uri = Uri.parse("content://" + FeedProvider.AUTHORITY + "/" + FeedProvider.PATH_CHANNEL + "/" + channel.getCurrentChannelId());
                            getContentResolver().delete(uri, null, null);
                            CurrentChannelLoader.loadCurrentChannel(DrawerActivity.this);
                        }
                    });
                    AlertDialog dialog = builder.create();
                    dialog.show();
                } else {
                    Toast.makeText(this, R.string.no_channel_available, Toast.LENGTH_SHORT).show();
                }
                return true;
            case R.id.action_manage_filters:
                if (CurrentChannelLoader.getInstance().getCurrentChannelId() != U.CHANNEL_ID_UNDEFINED) {
                    Intent intent = new Intent(this, FilterActivity.class);
                    startActivity(intent);
                } else {
                    Toast.makeText(this, R.string.no_channel_available, Toast.LENGTH_SHORT).show();
                }
                return true;
        }
        return super.onOptionsItemSelected(item);
    }

    // ****************************************
    // NAVIGATION DRAWER MENU
    // ****************************************

    @Override
    public boolean onNavigationItemSelected(@NonNull MenuItem item) {
        int id = item.getItemId();
        switch (id) {
            case R.id.nav_add_channel:
                AddChannelDialog dialog = new AddChannelDialog();
                dialog.show(getSupportFragmentManager(), "AddChannelDialog");
                break;
            case R.id.nav_settings:
                startActivity(new Intent(this, SettingsActivity.class));
                break;
            default:
                long currentChannelId = menuLoader.getChannelId(id);
                CurrentChannelLoader.getInstance().setCurrentChannelId(currentChannelId);
                CurrentChannelLoader.loadCurrentChannel(this);
        }
        drawerLayout.closeDrawer(GravityCompat.START);
        return true;
    }

    // ****************************************
    // DOWNLOAD DATA
    // ****************************************

    private void downloadChannelsData() {
        Intent serviceIntent = new Intent(this, DownloadService.class);
        serviceIntent.putExtra(U.EXTRA_CALLER, getClass().getName());
        serviceIntent.putExtra(U.EXTRA_SCHEDULE_NEXT_SERVICE, U.scheduleServiceIfNotBefore());
        startService(serviceIntent);
    }

    private void downloadChannelData(long channelId) {
        Intent serviceIntent = new Intent(this, DownloadService.class);
        serviceIntent.putExtra(U.EXTRA_CALLER, getClass().getName());
        serviceIntent.putExtra(U.EXTRA_CHANNEL_ID, channelId);
        serviceIntent.putExtra(U.EXTRA_SCHEDULE_NEXT_SERVICE, U.scheduleServiceIfNotBefore());
        startService(serviceIntent);
    }

    @Override
    public void onRefresh() {
        long currentChannelId = CurrentChannelLoader.getInstance().getCurrentChannelId();
        downloadChannelData(currentChannelId);
    }

    @Override
    public void onChannelLoaded(long channelId, String channelName, Date lastUpdate) {
        toolbar.setTitle(channelName);
        toolbar.setSubtitle(lastUpdate == null ? null :
                DateUtils.isToday(lastUpdate.getTime()) ? U.HH_mm.format(lastUpdate) : U.MMM_d_HH_mm.format(lastUpdate));
        swipeRefreshLayout.trySetEnabled(U.isConnected(this));
        btnAddChannel.setVisibility(View.GONE);
        if (searchMenuItem != null) {
            searchMenuItem.setEnabled(true);
        }
    }

    @Override
    public void onNoChannelLoaded() {
        toolbar.setTitle(getString(R.string.app_name));
        toolbar.setSubtitle(null);
        swipeRefreshLayout.trySetEnabled(false);
        btnAddChannel.setVisibility(View.VISIBLE);
        if (searchMenuItem != null) {
            searchMenuItem.setEnabled(false);
        }
    }

    // ****************************************
    // SETUP METHODS
    // ****************************************

    private void initDrawerLayout() {
        ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawerLayout, toolbar, R.string.empty_string, R.string.empty_string);
        drawerLayout.addDrawerListener(toggle);
        toggle.syncState();
        navigationView.setNavigationItemSelectedListener(this);
        menuLoader = new ChannelLoaderCallbacks(this, navigationView);
        getSupportLoaderManager().initLoader(MENU_LOADER_ID, null, menuLoader);
    }

    private void initToolbar() {
        toolbar.setTitle(getString(R.string.app_name));
        setSupportActionBar(toolbar);
    }

    private void initTabLayout() {
        tabAdapter = new TabPagerAdapter(getSupportFragmentManager(), getResources().getStringArray(R.array.tabs));
        tabPager.setAdapter(tabAdapter);
        tabLayout.setupWithViewPager(tabPager);
        tabPager.addOnPageChangeListener(new ViewPager.OnPageChangeListener() {
            @Override
            public void onPageScrolled(int position, float positionOffset, int positionOffsetPixels) {
            }

            @Override
            public void onPageSelected(int position) {
            }

            @Override
            public void onPageScrollStateChanged(int state) {
                swipeRefreshLayout.setEnabled(state == ViewPager.SCROLL_STATE_IDLE);
            }
        });
    }

    private void initSwipeRefreshLayout() {
        swipeRefreshLayout.setColorSchemeColors(ContextCompat.getColor(this, R.color.colorAccent));
        swipeRefreshLayout.setOnRefreshListener(this);
    }

    private void initBtnAddChannel() {
        btnAddChannel.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                AddChannelDialog dialog = new AddChannelDialog();
                dialog.show(getSupportFragmentManager(), "AddChannelDialog");
            }
        });
    }

}
