package sk.ikim23.montecarlo.core

import java.util.concurrent.atomic.AtomicReference
import kotlin.concurrent.thread

class Service<T>(private val task: IServiceTask<T>, private val renderer: IResultRenderer<T>,
                 private val skipResults: Int, private val maxResults: Int = 100) : IService {
    private val lock = Object()
    private val status = AtomicReference<Status>(Status.STOPPED)
    private val results = ArrayList<T>(maxResults)

    override fun isDone() = !task.hasNext()

    override fun render() {
        synchronized(lock) {
            renderer.render(results)
            results.clear()
            try {
                lock.notify()
            } catch (e: Exception) {
                e.printStackTrace()
            }
        }
    }

    override fun start() {
        val s = status.get()
        if (s == Status.PAUSED) status.set(Status.RUNNING)
        else if (s == Status.STOPPED) startThread()
    }

    override fun pause() {
        if (status.get() == Status.RUNNING) {
            status.set(Status.PAUSED)
        }
    }

    override fun stop() {
        if (status.get() != Status.STOPPED) {
            status.set(Status.STOPPED)
        }
    }

    private fun startThread() {
        thread(true, true) {
            results.clear()
            task.initialize()
            status.set(Status.RUNNING)
            var tickCount = 0
            while (task.hasNext() && status.get() != Status.STOPPED) {
                if (status.get() == Status.PAUSED) {
                    synchronized(lock) {
                        try {
                            lock.wait()
                        } catch (e: Exception) {
                            e.printStackTrace()
                        }
                    }
                }
                synchronized(lock) {
                    if (results.size >= maxResults) {
                        try {
                            lock.wait()
                        } catch (e: Exception) {
                            e.printStackTrace()
                        }
                    }
                    val result = task.tick()
                    if (tickCount++ % skipResults == 0) {
                        results.add(result)
                    }
                }
            }
            status.set(Status.STOPPED)
        }
    }
}