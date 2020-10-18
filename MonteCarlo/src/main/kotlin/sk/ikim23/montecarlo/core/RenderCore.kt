package sk.ikim23.montecarlo.core

import javafx.animation.AnimationTimer
import javafx.beans.property.SimpleObjectProperty
import java.util.*

class RenderCore {
    val statusProperty = SimpleObjectProperty<Status>(Status.STOPPED)
    private val services = LinkedList<IService>()
    private val timer = object : AnimationTimer() {
        override fun handle(now: Long) {
            synchronized(services) {
                if (statusProperty.value != Status.STOPPED) {
                    services.forEach { it.render() }
                    val allDone = services.all { it.isDone() }
                    if (allDone) {
                        statusProperty.set(Status.STOPPED)
                        stop() // timer.stop()
                    }
                }
            }
        }
    }

    init {
        timer.start()
    }

    fun registerService(service: IService) {
        synchronized(services) {
            services.add(service)
        }
    }

    fun clear() {
        synchronized(services) {
            services.forEach { it.stop() }
            services.clear()
        }
    }

    fun start() {
        if (statusProperty.value != Status.RUNNING) {
            statusProperty.set(Status.RUNNING)
            synchronized(services) {
                services.forEach { it.start() }
            }
            timer.start()
        }
    }

    fun pause() {
        if (statusProperty.value == Status.RUNNING) {
            statusProperty.set(Status.PAUSED)
            timer.stop()
            synchronized(services) {
                services.forEach { it.pause() }
            }
        }
    }

    fun stop() {
        if (statusProperty.value != Status.STOPPED) {
            statusProperty.set(Status.STOPPED)
            timer.stop()
            synchronized(services) {
                services.forEach { it.stop() }
            }
        }
    }
}