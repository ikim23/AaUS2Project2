package sk.ikim23.aircarrental.entity

import OSPABA.Simulation
import OSPStat.WStat
import sk.ikim23.aircarrental.sim.Config
import sk.ikim23.aircarrental.sim.IResettable
import sk.ikim23.aircarrental.stat.BusStats
import sk.ikim23.aircarrental.stat.WarmStat
import java.util.*

class Bus(val id: Int, val type: BusType, val mySim: Simulation) : IResettable {
    private val usage = WarmStat(WStat(mySim), mySim)
    private val passengers: Queue<Passenger> = LinkedList()
    private var nPassengers = 0
    private var nextDestination = Place.T1
    private var arrivalToDestination = .0
    private var roundCount = 0
    val destination get() = nextDestination

    init {
        usage.addSample(.0)
    }

    @Synchronized
    fun isEmpty() = passengers.isEmpty()

    @Synchronized
    fun freeSeats() = type.capacity - nPassengers

    @Synchronized
    fun add(passenger: Passenger) {
        if (passenger.size > freeSeats()) {
            throw IllegalArgumentException("$this does not have enough free seats for $passenger")
        }
        passengers.add(passenger)
        nPassengers += passenger.size
    }

    @Synchronized
    fun pop(): Passenger? {
        val passenger = passengers.poll()
        if (passenger != null) {
            nPassengers -= passenger.size
        }
        return passenger
    }

    @Synchronized
    fun nextDestination(destination: Place, arrivalTime: Double) {
        nextDestination = destination
        arrivalToDestination = arrivalTime
        if (nextDestination == Place.SERVICE_DESK) {
            usage.addSample(nPassengers.toDouble() / type.capacity)
            roundCount++
        }
    }

    @Synchronized
    fun stats(): BusStats {
        val interval = usage.confidenceInterval_90()
        return BusStats(id, distanceFromDestination(), type.capacity, nPassengers, roundCount, usage.mean(), interval[0], interval[1])
    }

    @Synchronized
    private fun distanceFromDestination(): String {
        val timeToArrival = arrivalToDestination - mySim.currentTime()
        if (timeToArrival <= 0) {
            return "arrived to $destination"
        }
        val kmToDestination = Config.secToKm(timeToArrival)
        return "${String.format("%.1f", kmToDestination)}km/${String.format("%.1f", timeToArrival)}s to $destination"
    }

    @Synchronized
    override fun toString() = "Bus(id=$id, destination=$destination, capacity=$nPassengers/${type.capacity}, round=$roundCount)"

    override fun reset() {
        passengers.clear()
        nPassengers = 0
        nextDestination = Place.T1
        arrivalToDestination = .0
        roundCount = 0
        usage.clear()
        usage.addSample(.0)
    }
}