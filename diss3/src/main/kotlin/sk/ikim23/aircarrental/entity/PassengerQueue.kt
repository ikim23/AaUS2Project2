package sk.ikim23.aircarrental.entity

import OSPABA.Simulation
import OSPStat.Stat
import OSPStat.WStat
import sk.ikim23.aircarrental.sim.IResettable
import sk.ikim23.aircarrental.stat.WarmStat
import java.util.*

class PassengerQueue(val mySim: Simulation) : IResettable {
    val queueSize = WarmStat(WStat(mySim), mySim)
    val waitTime = WarmStat(Stat(), mySim)
    private val queue: Queue<Passenger> = LinkedList()
    private var count = 0

    init {
        queueSize.addSample(.0)
    }

    fun add(passenger: Passenger?) {
        if (passenger == null) return
        passenger.queueTime = mySim.currentTime()
        queue.add(passenger)
        count += passenger.size
        queueSize.addSample(count.toDouble())
    }

    fun pop(): Passenger? {
        val passenger = queue.poll()
        if (passenger != null) {
            count -= passenger.size
            queueSize.addSample(count.toDouble())
            val waitingTime = mySim.currentTime() - passenger.queueTime
            waitTime.addSample(waitingTime)
        }
        return passenger
    }

    fun popMaxSize(maxSize: Int): Passenger? {
        val passenger = queue.firstOrNull { it.size <= maxSize }
        queue.remove(passenger)
        if (passenger != null) {
            count -= passenger.size
            queueSize.addSample(count.toDouble())
            val waitingTime = mySim.currentTime() - passenger.queueTime
            waitTime.addSample(waitingTime)
        }
        return passenger
    }

    override fun reset() {
        queue.clear()
        count = 0
        queueSize.clear()
        queueSize.addSample(count.toDouble())
        waitTime.clear()
    }
}