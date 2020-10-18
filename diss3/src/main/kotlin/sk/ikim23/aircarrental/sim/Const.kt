package sk.ikim23.aircarrental.sim

import OSPABA.MessageForm
import OSPABA.SimComponent
import OSPStat.Stat
import sk.ikim23.aircarrental.message.MyMessage

class ID {
    companion object {
        const val AGE_ACR = 1
        const val AGE_ARRIVAL_TERMINAL = 2
        const val AGE_BOSS = 3
        const val AGE_BUS = 4
        const val AGE_DEPARTURE_TERMINAL = 5
        const val AGE_OUTSIDE = 6
        const val AGE_SERVICE_DESK = 7

        const val MNG_ACR = 11
        const val MNG_ARRIVAL_TERMINAL = 12
        const val MNG_BOSS = 13
        const val MNG_BUS = 14
        const val MNG_DEPARTURE_TERMINAL = 15
        const val MNG_OUTSIDE = 16
        const val MNG_SERVICE_DESK = 17

        const val ASS_LOAD_ON_BUS_AT_ARRIVAL_TERMINAL = 201
        const val ASS_LOAD_ON_BUS_AT_SERVICE_DESK = 202
        const val ASS_MOVE_BUS = 203
        const val ASS_T1_ARRIVAL = 204
        const val ASS_T2_ARRIVAL = 205
        const val ASS_SERVICE_DESK_ARRIVAL = 206
        const val ASS_SERVE_PASSENGER = 207
        const val ASS_TAKE_OFF_BUS_AT_SERVICE_DESK = 208
        const val ASS_TAKE_OFF_BUS_AT_DEPARTURE_TERMINAL = 209
    }
}

class MessageNotImplementedException(msg: MessageForm, clazz: Any)
    : Exception("Response to message code ${msg.code()} is not implemented by ${clazz.javaClass.simpleName}")

fun withCast(simComponent: SimComponent, messageForm: MessageForm, func: (msg: MyMessage) -> Unit) {
//    println("${simComponent.javaClass.simpleName}: ${messageForm}")
    func(messageForm as MyMessage)
//    println("----------")
}

fun confidenceInterval(stat: Stat): DoubleArray {
    if (stat.sampleSize() > 2.0) {
        return stat.confidenceInterval_90()
    }
    return doubleArrayOf(.0, .0)
}

fun Double.format() = String.format("%.5f", this)

fun Double.toTime(): String {
    val hours = (this / (60 * 60)).toInt()
    var rest = this - (hours * 60 * 60)
    val minutes = (rest / 60).toInt()
    val seconds = rest - (minutes * 60)
    return "%02d:%02d:%08.5f".format(hours, minutes, seconds)
}

infix fun Double.safeDiv(divider: Int): Double {
    if (this == .0) {
        return .0
    }
    return this / divider
}