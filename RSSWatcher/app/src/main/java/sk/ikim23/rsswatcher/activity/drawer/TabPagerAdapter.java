package sk.ikim23.rsswatcher.activity.drawer;

import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentPagerAdapter;
import android.view.ViewGroup;

import sk.ikim23.rsswatcher.data.DbHelper;

public class TabPagerAdapter extends FragmentPagerAdapter {

    private final String[] pageTitles;
    private TabFragment currentFragment;

    public TabPagerAdapter(FragmentManager fragmentManager, String[] pageTitles) {
        super(fragmentManager);
        this.pageTitles = pageTitles;
    }

    @Override
    public Fragment getItem(int position) {
        switch (position) {
            case 0:
                return TabFragment.getInstance(position, "");
            case 1:
                return TabFragment.getInstance(position, "AND " + DbHelper.FEED_IS_READ + " = 0");
            case 2:
                return TabFragment.getInstance(position, "AND " + DbHelper.FEED_IS_ARCHIVED + " = 1");
        }
        return null;
    }

    @Override
    public CharSequence getPageTitle(int position) {
        return pageTitles[position];
    }

    @Override
    public int getCount() {
        return pageTitles.length;
    }

    @Override
    public void setPrimaryItem(ViewGroup container, int position, Object object) {
        if (getCurrentFragment() != object) {
            currentFragment = ((TabFragment) object);
        }
        super.setPrimaryItem(container, position, object);
    }

    public TabFragment getCurrentFragment() {
        return currentFragment;
    }
}
