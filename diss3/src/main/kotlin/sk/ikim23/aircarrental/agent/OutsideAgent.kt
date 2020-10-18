package sk.ikim23.aircarrental.agent

import OSPABA.Agent
import OSPABA.Simulation
import OSPStat.Stat
import sk.ikim23.aircarrental.sim.ID
import sk.ikim23.aircarrental.M
import sk.ikim23.aircarrental.assistant.PassengerArrivalSharedAssistant
import sk.ikim23.aircarrental.entity.Place
import sk.ikim23.aircarrental.manager.OutsideManager
import sk.ikim23.aircarrental.sim.Config
import sk.ikim23.aircarrental.sim.ResettableAgent
import sk.ikim23.aircarrental.stat.CounterStat
import sk.ikim23.aircarrental.stat.WarmStat

class OutsideAgent(mySim: Simulation, parent: Agent) : ResettableAgent(ID.AGE_OUTSIDE, mySim, parent) {
    private var stopArrivalsAtTime = .0
    val sysTimeArrivingPassenger = WarmStat(Stat(), mySim)
    val sysTimeLeavingPassenger = WarmStat(Stat(), mySim)
    var totalGenerated = 0
    var totalReturned = 0
    var t1PassengerCount = CounterStat(mySim)
    var t2PassengerCount = CounterStat(mySim)
    var serviceDeskPassengerCount = CounterStat(mySim)

    init {
        OutsideManager(mySim, this)
        PassengerArrivalSharedAssistant(mySim, this, ID.ASS_T1_ARRIVAL, Place.T1, Config.t1Arrival, Config.passengerSize)
        PassengerArrivalSharedAssistant(mySim, this, ID.ASS_T2_ARRIVAL, Place.T2, Config.t2Arrival, Config.passengerSize)
        PassengerArrivalSharedAssistant(mySim, this, ID.ASS_SERVICE_DESK_ARRIVAL, Place.SERVICE_DESK, Config.serviceDeskArrival, Config.passengerSize)
        addOwnMessage(M.INIT)
        addOwnMessage(M.PASSENGER_ARRIVED)
        addOwnMessage(M.PASSENGER_LEAVES_SYSTEM)
    }

    fun canProduceNextArrival() = mySim().currentTime() < stopArrivalsAtTime

    override fun reset() {
        super.reset()
        sysTimeArrivingPassenger.clear()
        sysTimeLeavingPassenger.clear()
        totalGenerated = 0
        totalReturned = 0
        t1PassengerCount.clear()
        t2PassengerCount.clear()
        serviceDeskPassengerCount.clear()
    }

    fun init(stopArrivals: Double) {
        stopArrivalsAtTime = stopArrivals
    }
}