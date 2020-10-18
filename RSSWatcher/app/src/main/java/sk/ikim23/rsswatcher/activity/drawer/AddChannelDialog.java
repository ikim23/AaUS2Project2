package sk.ikim23.rsswatcher.activity.drawer;

import android.app.Dialog;
import android.content.Context;
import android.content.DialogInterface;
import android.database.Cursor;
import android.net.Uri;
import android.os.Bundle;
import android.support.annotation.NonNull;
import android.support.v4.app.DialogFragment;
import android.support.v7.app.AlertDialog;
import android.view.LayoutInflater;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.webkit.URLUtil;
import android.widget.EditText;

import sk.ikim23.rsswatcher.R;
import sk.ikim23.rsswatcher.data.DbHelper;
import sk.ikim23.rsswatcher.data.FeedProvider;

public class AddChannelDialog extends DialogFragment {

    public interface OnChannelAddListener {
        void onChannelAdd(String channelName, String channelUrl);
    }

    private OnChannelAddListener listener;

    @Override
    public void onAttach(Context context) {
        super.onAttach(context);
        listener = (OnChannelAddListener) context;
    }

    @NonNull
    @Override
    public Dialog onCreateDialog(Bundle savedInstanceState) {
        AlertDialog.Builder builder = new AlertDialog.Builder(getActivity());
        LayoutInflater inflater = getActivity().getLayoutInflater();
        View view = inflater.inflate(R.layout.dialog_add_channel, null);
        final EditText channelName = (EditText) view.findViewById(R.id.channel_name);
        final EditText channelUrl = (EditText) view.findViewById(R.id.channel_url);
        builder
                .setTitle(R.string.new_channel)
                .setView(view)
                .setPositiveButton(R.string.add, new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                    }
                })
                .setNegativeButton(R.string.cancel, new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        dialog.dismiss();
                    }
                });
        final AlertDialog dialog = builder.create();
        dialog.show();
        dialog.getButton(AlertDialog.BUTTON_POSITIVE).setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                String name = channelName.getText().toString();
                String url = channelUrl.getText().toString();
                boolean accept = true;
                Context ctx = getContext();
                if (name.length() == 0) {
                    channelName.setError(getString(R.string.dialog_add_channel_invalid_name));
                    accept = false;
                }
                if (url.length() == 0) {
                    channelUrl.setError(getString(R.string.dialog_add_channel_invalid_url));
                    accept = false;
                } else {
                    if (!url.startsWith("http")) {
                        url = "http://" + url;
                    }
                    if (!URLUtil.isValidUrl(url)) {
                        channelUrl.setError(getString(R.string.dialog_add_channel_invalid_url));
                        accept = false;
                    }
                    Uri uri = Uri.parse("content://" + FeedProvider.AUTHORITY + "/" + FeedProvider.PATH_CHANNEL);
                    Cursor cursor = ctx.getContentResolver().query(uri, null, DbHelper.CHANNEL_URL + "='" + url + "'", null, null, null);
                    if (cursor != null) {
                        if (cursor.getCount() > 0) {
                            channelUrl.setError(getString(R.string.dialog_add_channel_invalid_url_exists));
                            accept = false;
                        }
                        cursor.close();
                    }
                }
                if (accept) {
                    listener.onChannelAdd(name, url);
                    dialog.dismiss();
                }
            }
        });
        return dialog;
    }

    @Override
    public void onActivityCreated(Bundle savedInstanceState) {
        super.onActivityCreated(savedInstanceState);
        Window window = getDialog().getWindow();
        if (window != null) {
            window.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_STATE_ALWAYS_VISIBLE);
        }
    }

}
