package sk.ikim23.carrental.core

import java.text.DecimalFormat

abstract class Event(private val core: Core, val execTime: Double) : Comparable<Event> {
    companion object {
        val fmt = DecimalFormat("00000.00000000")
    }

    abstract fun exec()

    fun log(message: String) {
        if (core.log) println("${fmt.format(execTime)} | ${javaClass.simpleName.padEnd(25, ' ')} | $message")
    }

    override fun compareTo(other: Event) = execTime.compareTo(other.execTime)
}