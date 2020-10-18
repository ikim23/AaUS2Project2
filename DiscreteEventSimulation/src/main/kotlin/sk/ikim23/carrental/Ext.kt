package sk.ikim23.carrental

fun Int.times(call: (i: Int) -> Unit) {
    for (index in 0 until this) call(index)
}

fun sqrt(i: Int) = Math.sqrt(i.toDouble())

fun withTryCatch(call: () -> Unit) {
    try {
        call()
    } catch (e: Exception) {
        e.printStackTrace()
    }
}

infix fun Double.safeDiv(divider: Int): Double {
    val result = this / divider
    return if (result.isNaN()) 0.0 else result
}

infix fun Double.safeDiv(divider: Double): Double {
    val result = this / divider
    return if (result.isNaN()) 0.0 else result
}

fun daysToSec(days: Int) = (days * 24 * 60 * 60).toDouble()

fun formatTime(time: Double): String {
    val days = (time / (24 * 60 * 60)).toInt()
    var rest = time - (days * 24 * 60 * 60)
    val hours = (rest / (60 * 60)).toInt()
    rest -= hours * 60 * 60
    val minutes = (rest / 60).toInt()
    val seconds = rest - (minutes * 60)
    return "%02d:%02d:%02d:%06.3f".format(days, hours, minutes, seconds)
}
