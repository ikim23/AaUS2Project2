package sk.ikim23.carrental.core

import sk.ikim23.carrental.withTryCatch
import java.util.*
import java.util.concurrent.atomic.AtomicReference

abstract class Pauseable(private val log: Boolean = false) {
    enum class Status {
        RUNNING, PAUSED, STOPPED, SLEEPING, DESTROYED
    }

    companion object {
        var ID = 0
        private val threads = LinkedList<Pauseable>()

        fun destroyAll() {
            threads.forEach { it.destroy() }
            threads.clear()
        }
    }

    private val threadStatus = AtomicReference(Status.STOPPED)
    private val lock = Object()
    private var sleep = 0L
    val status: Status get() = threadStatus.get()

    init {
        val thread = Thread({
            while (true) {
                if (status == Status.SLEEPING) {
                    withTryCatch {
                        Thread.sleep(sleep)
                    }
                    if (status == Status.SLEEPING) {
                        threadStatus.set(Status.RUNNING)
                    }
                }
                if (status == Status.PAUSED || status == Status.STOPPED) {
                    synchronized(lock) {
                        withTryCatch {
                            lock.wait()
                        }
                    }
                }
                if (status == Status.DESTROYED) {
                    return@Thread
                }
                if (canTick()) {
                    tick()
                } else {
                    stop()
                    afterDone()
                }
            }
        })
        thread.name = "SimCoreWorker-${++ID}"
        thread.start()
        threads.add(this)
    }

    open fun canTick() = true

    abstract fun tick()

    abstract fun beforeStart()

    abstract internal fun afterDone()

    fun sleep(millis: Long) {
        if (millis > 0 && status == Status.RUNNING) {
            sleep = millis
            threadStatus.set(Status.SLEEPING)
        }
    }

    fun start() {
        val s = status
        if (s == Status.DESTROYED) {
            throw IllegalStateException("Destroyed instance can not be started")
        } else if (s == Status.PAUSED || s == Status.STOPPED) {
            if (s == Status.STOPPED) beforeStart()
            threadStatus.set(Status.RUNNING)
            synchronized(lock) {
                withTryCatch {
                    lock.notifyAll()
                }
            }
            if (log) println(status)
        }
    }

    fun pause() {
        val s = status
        if (s != Status.DESTROYED && (s == Status.RUNNING || s == Status.SLEEPING)) {
            threadStatus.set(Status.PAUSED)
            if (log) println(status)
        }
    }

    fun stop() {
        val s = status
        if (s != Status.DESTROYED && s != Status.STOPPED) {
            threadStatus.set(Status.STOPPED)
            synchronized(lock) {
                withTryCatch {
                    lock.notifyAll()
                }
            }
            if (log) println(status)
        }
    }

    fun destroy() {
        if (status != Status.DESTROYED) {
            threadStatus.set(Status.DESTROYED)
            synchronized(lock) {
                withTryCatch {
                    lock.notifyAll()
                }
            }
            if (log) println(status)
        }
    }
}