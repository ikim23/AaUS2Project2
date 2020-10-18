package sk.ikim23.aircarrental.entity

import OSPABA.Simulation
import OSPStat.WStat
import sk.ikim23.aircarrental.sim.IResettable
import sk.ikim23.aircarrental.stat.ServiceDeskStats
import sk.ikim23.aircarrental.stat.WarmStat

class ServiceDesk(val id: Int, val mySim: Simulation) : IResettable {
    val usage = WarmStat(WStat(mySim), mySim)
    private var servedPassenger: Passenger? = null

    init {
        usage.addSample(.0)
    }

    fun isFree() = servedPassenger == null

    fun serve(passenger: Passenger) {
        if (!isFree()) throw IllegalStateException("Service desk cannot serve more that one passenger")
        servedPassenger = passenger
        usage.addSample(1.0)
    }

    fun free(): Passenger {
        if (isFree()) throw IllegalStateException("Service desk is already free")
        val passenger = servedPassenger
        servedPassenger = null
        usage.addSample(.0)
        return passenger!!
    }

    fun stats(): ServiceDeskStats {
        val passenger = if (servedPassenger != null) "Passenger ${servedPassenger?.id}" else "free"
        val interval = usage.confidenceInterval_90()
        return ServiceDeskStats(id, passenger, usage.mean(), interval[0], interval[1])
    }

    override fun reset() {
        servedPassenger = null
        usage.clear()
        usage.addSample(.0)
    }
}