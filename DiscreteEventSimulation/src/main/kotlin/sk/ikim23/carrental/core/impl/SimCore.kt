package sk.ikim23.carrental.core.impl

import sk.ikim23.carrental.core.Core
import sk.ikim23.carrental.core.Event
import sk.ikim23.carrental.core.obj.Bus
import sk.ikim23.carrental.core.obj.Customer
import sk.ikim23.carrental.core.obj.ServiceDesk
import sk.ikim23.carrental.core.obj.StatsQueue
import sk.ikim23.carrental.random.ExpRandom
import sk.ikim23.carrental.random.NormRandom
import sk.ikim23.carrental.times
import java.util.*

class SimCore(val listener: ISimListener) : Core() {
    val tTimeToT1 = calcTravelTime(6.4)
    val tTimeToT2 = calcTravelTime(0.5)
    val tTimeToRental = calcTravelTime(2.5)
    // Randoms
    val rArrivalToT1 = ExpRandom(43.0 / (60 * 60))
    val rArrivalToT2 = ExpRandom(19.0 / (60 * 60))
    val rLoadOnT1 = NormRandom(10.0, 14.0)
    val rLoadOnT2 = NormRandom(10.0, 14.0)
    val rLeavedBus = NormRandom(4.0, 12.0)
    val rService = NormRandom(2.0 * 60, 10.0 * 60)
    // Model objects
    val t1Queue = StatsQueue<Customer>(this)
    val t2Queue = StatsQueue<Customer>(this)
    val rentalQueue = StatsQueue<Customer>(this)
    lateinit var serviceDesk: ServiceDesk
    var nBuses = 0
    val buses = LinkedList<Bus>()
    val stats = Stats(this)

    fun customersAreWaiting() = hasTime() || !t1Queue.isEmpty() || !t2Queue.isEmpty()

    fun init(endTime: Double, nBuses: Int, nDesks: Int) {
        super.init(endTime)
        this.nBuses = nBuses
        serviceDesk = ServiceDesk(this, nDesks)
    }

    override fun beforeStart() {
        super.beforeStart()
        val rand = Random()
        rArrivalToT1.setSeed(rand.nextLong())
        rArrivalToT2.setSeed(rand.nextLong())
        rLoadOnT1.setSeed(rand.nextLong())
        rLoadOnT2.setSeed(rand.nextLong())
        rLeavedBus.setSeed(rand.nextLong())
        rService.setSeed(rand.nextLong())
        t1Queue.clear()
        t2Queue.clear()
        rentalQueue.clear()
        serviceDesk.clear()
        buses.clear()
        stats.clear()
        // init model
        nBuses.times {
            val bus = Bus()
            buses.add(bus)
            addEvent(randArrival(bus))
        }
        addEvent(CstArrivedOnT1Event(this))
        addEvent(CstArrivedOnT2Event(this))
        addEvent(SlowMoEvent(this, listener))
    }

    override fun afterDone() {
        listener.onDone(stats)
    }

    private fun randArrival(bus: Bus): Event {
        val maxDelay = 0.5
        val rand = Random()
        return when (rand.nextInt(3)) {
            0 -> BusArrivedToRentalEvent(this, bus, rand.nextDouble() * maxDelay)
            1 -> BusArrivedToT1Event(this, bus, rand.nextDouble() * maxDelay)
            2 -> BusArrivedToT2Event(this, bus, rand.nextDouble() * maxDelay)
            else -> throw IllegalArgumentException()
        }
    }

    private fun calcTravelTime(distance: Double) = (distance / 35) * (60 * 60)
}