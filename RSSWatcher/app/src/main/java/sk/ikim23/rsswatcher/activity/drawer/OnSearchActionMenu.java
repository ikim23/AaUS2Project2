package sk.ikim23.rsswatcher.activity.drawer;

import android.support.v4.view.MenuItemCompat;
import android.support.v7.widget.SearchView;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;

import sk.ikim23.rsswatcher.U;
import sk.ikim23.rsswatcher.data.DbHelper;

public class OnSearchActionMenu implements MenuItemCompat.OnActionExpandListener, SearchView.OnQueryTextListener {

    private final DrawerActivity activity;
    private final Menu menu;
    private final MenuItem searchMenuItem;

    public OnSearchActionMenu(DrawerActivity activity, Menu menu, MenuItem searchMenuItem) {
        this.activity = activity;
        this.menu = menu;
        this.searchMenuItem = searchMenuItem;
    }

    @Override
    public boolean onMenuItemActionExpand(MenuItem item) {
        activity.setViewsEnabled(false);
        setMenuItemVisible(menu, searchMenuItem, false);
        activity.setBulkActionEnabled(false);
        return true;
    }

    @Override
    public boolean onMenuItemActionCollapse(MenuItem item) {
        activity.setViewsEnabled(true);
        setMenuItemVisible(menu, searchMenuItem, true);
        activity.setMatchQuery(null);
        activity.setBulkActionEnabled(true);
        return true;
    }

    private void setMenuItemVisible(Menu menu, MenuItem searchMenuItem, boolean visible) {
        MenuItem menuItem;
        for (int i = 0; i < menu.size(); i++) {
            menuItem = menu.getItem(i);
            if (menuItem != searchMenuItem) {
                menuItem.setVisible(visible);
            }
        }
    }

    @Override
    public boolean onQueryTextSubmit(String query) {
        Log.d(getClass().getName(), "original query: " + query);
        query = U.normalize(query);
        Log.d(getClass().getName(), "normalized query: " + query);
        String matchQuery = String.format(" AND %s IN (SELECT docid FROM %s WHERE %s MATCH '%s')",
                DbHelper.FEED_ID,
                DbHelper.TABLE_FEED_FTS,
                DbHelper.TABLE_FEED_FTS, query);
        activity.setMatchQuery(matchQuery);
        return true;
    }

    @Override
    public boolean onQueryTextChange(String newText) {
        return true;
    }
}
