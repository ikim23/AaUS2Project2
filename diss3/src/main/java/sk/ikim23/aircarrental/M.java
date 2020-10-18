package sk.ikim23.aircarrental;

import java.lang.reflect.Field;

public class M {
    public static final int INIT = 100;
    public static final int PASSENGER_ARRIVED = 101;
    public static final int MOVE_BUS = 102;
    public static final int BUS_ARRIVED = 103;
    public static final int MOVE_BUS_TO_NEXT_DESTINATION = 104;
    public static final int LOAD_PASSENGER = 105;
    public static final int LOAD_PASSENGER_DONE = 106;
    public static final int BUS_LOADED = 107;
    public static final int MOVE_BUS_DONE = 108;
    public static final int TAKE_OFF_FROM_BUS = 109;
    public static final int TAKE_OFF_FROM_BUS_DONE = 110;
    public static final int BUS_EMPTIED = 111;
    public static final int PROCESS_PASSENGER_FROM_BUS = 112;
    public static final int SERVE_PASSENGER = 113;
    public static final int SERVE_PASSENGER_DONE = 114;
    public static final int QUEUE_PASSENGER_TO_T3 = 115;
    public static final int PASSENGER_LEAVES_SYSTEM = 116;

    private static final M instance = new M();

    public static String getMessageName(int code) {
        Field[] fields = M.class.getFields();
        for (Field field : fields) {
            try {
                int value = field.getInt(instance);
                if (value == code) {
                    return field.getName();
                }
            } catch (IllegalAccessException e) {
                e.printStackTrace();
            }
        }
        return Integer.toString(code);
    }
}
