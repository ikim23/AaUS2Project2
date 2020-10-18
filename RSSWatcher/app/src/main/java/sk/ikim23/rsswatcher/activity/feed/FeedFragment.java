package sk.ikim23.rsswatcher.activity.feed;

import android.os.Bundle;
import android.support.annotation.Nullable;
import android.support.v4.app.Fragment;
import android.support.v4.widget.NestedScrollView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.webkit.WebChromeClient;
import android.webkit.WebView;

import java.net.MalformedURLException;
import java.net.URL;
import java.util.HashSet;
import java.util.Set;

import sk.ikim23.rsswatcher.R;
import sk.ikim23.rsswatcher.U;

public class FeedFragment extends Fragment {

    static final Set<Long> loadedWeb = new HashSet<>();
    private static final String HTML = "<div style=\"star_background-color: white; padding: 10px;\"><h1 style=\"font-size: 1.5em\">%s</h1><p style=\"font-size: 0.8em; text-align: right;\">%s</p>%s</div>";
    private long id;
    private String feedLink;
    private boolean isFeedDisplayed;
    private String html;
    private String baseUrl;
    private WebView webView;
    private NestedScrollView scrollView;

    static FeedFragment newInstance(long id, String title, String date, String description, String link) {
        Bundle bundle = new Bundle();
        bundle.putLong(U.EXTRA_ID, id);
        bundle.putString(U.EXTRA_TITLE, title);
        bundle.putString(U.EXTRA_DATE, date);
        bundle.putString(U.EXTRA_DESCRIPTION, description);
        bundle.putString(U.EXTRA_LINK, link);
        FeedFragment instance = new FeedFragment();
        instance.setArguments(bundle);
        return instance;
    }

    @Nullable
    @Override
    public View onCreateView(LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        ViewGroup rootView = (ViewGroup) inflater.inflate(R.layout.fragment_feed, container, false);
        Bundle arguments = getArguments();
        if (arguments != null) {
            String title = arguments.getString(U.EXTRA_TITLE);
            String date = arguments.getString(U.EXTRA_DATE);
            String description = arguments.getString(U.EXTRA_DESCRIPTION);
            id = arguments.getLong(U.EXTRA_ID, -1L);
            feedLink = arguments.getString(U.EXTRA_LINK);

            html = String.format(HTML, title, date, description != null ? description : "");
            baseUrl = null;
            try {
                URL url = new URL(feedLink);
                baseUrl = url.getProtocol() + "://" + url.getHost();
            } catch (MalformedURLException e) {
                e.printStackTrace();
                baseUrl = U.ASSETS_BASE_URL;
            }
            webView = (WebView) rootView.findViewById(R.id.web_view);
            scrollView = (NestedScrollView) rootView.findViewById(R.id.scroll_view);
            scrollView.setOnTouchListener(new NestedScrollViewTouchListener(scrollView, webView));
            showFeed();
        }
        return rootView;
    }

    @Override
    public void onDestroy() {
        super.onDestroy();
        loadedWeb.remove(id);
    }

    public void showFeed() {
        webView.getSettings().setJavaScriptEnabled(false);
        webView.loadDataWithBaseURL(baseUrl, html, "text/html", "utf-8", null);
        isFeedDisplayed = true;
        loadedWeb.remove(id);
    }

    public boolean showWeb() {
        if (feedLink == null) {
            FeedActivity.showBadLinkAlert(getContext());
            return false;
        }
        final FeedActivity activity = (FeedActivity) getActivity();
        webView.getSettings().setJavaScriptEnabled(true);
        activity.setBarProgressVisibility(View.VISIBLE);
        webView.setWebChromeClient(new WebChromeClient() {
            @Override
            public void onProgressChanged(WebView view, int newProgress) {
                super.onProgressChanged(view, newProgress);
                activity.setBarProgress(newProgress);
                if (newProgress > 95) {
                    activity.setBarProgressVisibility(View.GONE);
                    webView.setWebChromeClient(new WebChromeClient());
                }
            }
        });
        webView.loadUrl(feedLink);
        isFeedDisplayed = false;
        loadedWeb.add(id);
        return true;
    }

    public boolean isFeedDisplayed() {
        return isFeedDisplayed;
    }

}
