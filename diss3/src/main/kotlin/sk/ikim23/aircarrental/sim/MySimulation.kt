package sk.ikim23.aircarrental.sim

import OSPABA.Simulation
import sk.ikim23.aircarrental.agent.*
import sk.ikim23.aircarrental.entity.BusType
import sk.ikim23.aircarrental.entity.Place
import sk.ikim23.aircarrental.stat.CostStats
import sk.ikim23.aircarrental.stat.GlobalStats
import sk.ikim23.aircarrental.stat.SimStats

class MySimulation : Simulation(), ISimulationTime {
    private val boss = BossAgent(this)
    private val outside = OutsideAgent(this, boss)
    private val acr = ACRAgent(this, boss)
    private val arrivalTerminal = ArrivalTerminalAgent(this, acr)
    private val departureTerminal = DepartureTerminalAgent(this, acr)
    internal val serviceDesk = ServiceDeskAgent(this, acr)
    internal val bus = BusAgent(this, acr)
    private val gStats = GlobalStats(this)

    fun configure(nEmployees: Int, nBuses: Int, type: BusType, stopArrivalsAtTime: Double) {
        serviceDesk.init(nEmployees)
        bus.init(nBuses, type)
        outside.init(stopArrivalsAtTime)
    }

    override fun prepareSimulation() {
        super.prepareSimulation()
        Config.reset()
        gStats.reset()
    }

    override fun prepareReplication() {
        super.prepareReplication()
        boss.reset()
    }

    override fun replicationFinished() {
        gStats.time.addSample(currentTime() - Config.warmTime)
        gStats.t1ArrivalCount += outside.t1PassengerCount.count()
        gStats.t2ArrivalCount += outside.t2PassengerCount.count()
        gStats.serviceDeskArrivalCount += outside.serviceDeskPassengerCount.count()
        gStats.sysTimeArrivingPassenger.addSamples(outside.sysTimeArrivingPassenger.stat)
        gStats.sysTimeLeavingPassenger.addSamples(outside.sysTimeLeavingPassenger.stat)
        gStats.queueSizeT1.addSamples(arrivalTerminal.terminals[Place.T1]?.queueSize?.stat)
        gStats.queueSizeT2.addSamples(arrivalTerminal.terminals[Place.T2]?.queueSize?.stat)
        gStats.queueSizeService.addSamples(serviceDesk.qToService.queueSize.stat)
        gStats.queueSizeToT3.addSamples(serviceDesk.qToT3.queueSize.stat)
        gStats.waitTimeT1.addSamples(arrivalTerminal.terminals[Place.T1]?.waitTime?.stat)
        gStats.waitTimeT2.addSamples(arrivalTerminal.terminals[Place.T2]?.waitTime?.stat)
        gStats.waitTimeService.addSamples(serviceDesk.qToService.waitTime.stat)
        gStats.waitTimeToT3.addSamples(serviceDesk.qToT3.waitTime.stat)
        val cStats = CostStats(this)
        gStats.drivenKm.addSample(cStats.drivenKm)
        gStats.driveCosts.addSample(cStats.driveCosts)
        gStats.driverCosts.addSample(cStats.driverCosts)
        gStats.employeeCosts.addSample(cStats.employeeCosts)
        super.replicationFinished()
    }

    fun replStats(): SimStats {
        val cStats = CostStats(this)
        val buses = bus.buses.map { it.stats() }
        val desks = serviceDesk.workers.map { it.stats() }
        return SimStats(
                Math.max(currentTime() - Config.warmTime, .0),
                outside.t1PassengerCount.count(),
                outside.t2PassengerCount.count(),
                outside.serviceDeskPassengerCount.count(),
                outside.sysTimeArrivingPassenger,
                outside.sysTimeLeavingPassenger,
                arrivalTerminal.terminals[Place.T1]?.queueSize!!,
                arrivalTerminal.terminals[Place.T2]?.queueSize!!,
                serviceDesk.qToService.queueSize,
                serviceDesk.qToT3.queueSize,
                arrivalTerminal.terminals[Place.T1]?.waitTime!!,
                arrivalTerminal.terminals[Place.T2]?.waitTime!!,
                serviceDesk.qToService.waitTime,
                serviceDesk.qToT3.waitTime,
                cStats.drivenKm,
                cStats.driveCosts,
                cStats.driverCosts,
                cStats.employeeCosts,
                buses,
                desks
        )
    }

    fun globStats() = gStats.stats()
}