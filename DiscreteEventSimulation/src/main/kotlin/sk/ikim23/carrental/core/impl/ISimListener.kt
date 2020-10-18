package sk.ikim23.carrental.core.impl

interface ISimListener {
    enum class Step(val title: String, val value: Int) {
        SECOND("second", 1),
        MINUTE("minute", 60),
        HOUR("hour", 60 * 60),
        DAY("day", 24 * 60 * 60),
        NONE("none", 24 * 60 * 60)
    }

    val timeStep: Step

    fun onDone(stats: Stats)
    fun onStep(stats: Stats)
}