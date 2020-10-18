package sk.ikim23.carrental.core

import sk.ikim23.carrental.core.impl.SlowMoEvent
import java.util.*

abstract class Core(val log: Boolean = false) : Pauseable(log), ITime {
    override val currentTime get() = curTime
    private val eventQueue = PriorityQueue<Event>()
    private var endTime = 0.0
    private var curTime = 0.0

    override fun tick() {
        val event = eventQueue.poll()
        curTime = event.execTime
        event.exec()
    }

    override fun beforeStart() {
        curTime = 0.0
        eventQueue.clear()
    }

    override fun canTick() = hasTime() || hasEvents()

    fun hasTime() = curTime <= endTime

    fun hasEvents() = eventQueue.isNotEmpty()

    fun addEvent(event: Event) = eventQueue.add(event)

    fun init(endTime: Double) {
        this.endTime = endTime
    }
}
