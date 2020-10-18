package sk.ikim23.aircarrental.stat

import OSPABA.Simulation
import OSPStat.Stat
import OSPStat.WStat
import sk.ikim23.aircarrental.sim.IResettable

class GlobalStats(mySim: Simulation) : IResettable {
    val time = Stat()
    var t1ArrivalCount = 0
    var t2ArrivalCount = 0
    var serviceDeskArrivalCount = 0
    val sysTimeArrivingPassenger = Stat()
    val sysTimeLeavingPassenger = Stat()
    val queueSizeT1 = WStat(mySim)
    val queueSizeT2 = WStat(mySim)
    val queueSizeService = WStat(mySim)
    val queueSizeToT3 = WStat(mySim)
    val waitTimeT1 = Stat()
    val waitTimeT2 = Stat()
    val waitTimeService = Stat()
    val waitTimeToT3 = Stat()
    val drivenKm = Stat()
    val driveCosts = Stat()
    val driverCosts = Stat()
    val employeeCosts = Stat()

    fun stats(): SimStats {
        return SimStats(
                time.mean(),
                t1ArrivalCount,
                t2ArrivalCount,
                serviceDeskArrivalCount,
                sysTimeArrivingPassenger,
                sysTimeLeavingPassenger,
                queueSizeT1,
                queueSizeT2,
                queueSizeService,
                queueSizeToT3,
                waitTimeT1,
                waitTimeT2,
                waitTimeService,
                waitTimeToT3,
                drivenKm.mean(),
                driveCosts.mean(),
                driverCosts.mean(),
                employeeCosts.mean()
        )
    }

    override fun reset() {
        time.clear()
        t1ArrivalCount = 0
        t2ArrivalCount = 0
        serviceDeskArrivalCount = 0
        sysTimeArrivingPassenger.clear()
        sysTimeLeavingPassenger.clear()
        queueSizeT1.clear()
        queueSizeT2.clear()
        queueSizeService.clear()
        queueSizeToT3.clear()
        waitTimeT1.clear()
        waitTimeT2.clear()
        waitTimeService.clear()
        waitTimeToT3.clear()
        drivenKm.clear()
        driveCosts.clear()
        driverCosts.clear()
        employeeCosts.clear()
    }
}