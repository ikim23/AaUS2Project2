package sk.ikim23.aircarrental.stat

import OSPStat.Stat
import sk.ikim23.aircarrental.sim.confidenceInterval

class SimStats(
        val time: Double,
        val t1ArrivalCount: Int,
        val t2ArrivalCount: Int,
        val serviceDeskArrivalCount: Int,
        systemTimeArrivingPassenger: Stat,
        systemTimeLeavingPassenger: Stat,
        queueSizeT1: Stat,
        queueSizeT2: Stat,
        queueSizeService: Stat,
        queueSizeToT3: Stat,
        waitTimeT1: Stat,
        waitTimeT2: Stat,
        waitTimeService: Stat,
        waitTimeToT3: Stat,
        drivenKm: Double,
        driveCosts: Double,
        driverCosts: Double,
        employeeCosts: Double,
        busStats: List<BusStats> = listOf(),
        serviceDeskStats: List<ServiceDeskStats> = listOf()
) {

    val avgSystemTimeArrivingPassenger = systemTimeArrivingPassenger.mean()
    val lowSystemTimeArrivingPassenger: Double
    val uppSystemTimeArrivingPassenger: Double
    val avgSystemTimeLeavingPassenger = systemTimeLeavingPassenger.mean()
    val lowSystemTimeLeavingPassenger: Double
    val uppSystemTimeLeavingPassenger: Double
    val avgQueueSizeT1 = queueSizeT1.mean()
    val lowQueueSizeT1: Double
    val uppQueueSizeT1: Double
    val avgQueueSizeT2 = queueSizeT2.mean()
    val lowQueueSizeT2: Double
    val uppQueueSizeT2: Double
    val avgQueueSizeService = queueSizeService.mean()
    val lowQueueSizeService: Double
    val uppQueueSizeService: Double
    val avgQueueSizeToT3 = queueSizeToT3.mean()
    val lowQueueSizeToT3: Double
    val uppQueueSizeToT3: Double
    val avgWaitTimeT1 = waitTimeT1.mean()
    val lowWaitTimeT1: Double
    val uppWaitTimeT1: Double
    val avgWaitTimeT2 = waitTimeT2.mean()
    val lowWaitTimeT2: Double
    val uppWaitTimeT2: Double
    val avgWaitTimeService = waitTimeService.mean()
    val lowWaitTimeService: Double
    val uppWaitTimeService: Double
    val avgWaitTimeToT3 = waitTimeToT3.mean()
    val lowWaitTimeToT3: Double
    val uppWaitTimeToT3: Double
    val drivenKm = drivenKm
    val driveCosts = driveCosts
    val driverCosts = driverCosts
    val employeeCosts = employeeCosts
    val busStats = busStats
    val serviceDeskStats = serviceDeskStats

    init {
        val intSysTimeArrivingPassenger = confidenceInterval(systemTimeArrivingPassenger)
        lowSystemTimeArrivingPassenger = intSysTimeArrivingPassenger[0]
        uppSystemTimeArrivingPassenger = intSysTimeArrivingPassenger[1]
        val intSysTimeLeavingPassenger = confidenceInterval(systemTimeLeavingPassenger)
        lowSystemTimeLeavingPassenger = intSysTimeLeavingPassenger[0]
        uppSystemTimeLeavingPassenger = intSysTimeLeavingPassenger[1]
        val intQueueSizeT1 = confidenceInterval(queueSizeT1)
        lowQueueSizeT1 = intQueueSizeT1[0]
        uppQueueSizeT1 = intQueueSizeT1[0]
        val intQueueSizeT2 = confidenceInterval(queueSizeT2)
        lowQueueSizeT2 = intQueueSizeT2[0]
        uppQueueSizeT2 = intQueueSizeT2[0]
        val intQueueSizeService = confidenceInterval(queueSizeService)
        lowQueueSizeService = intQueueSizeService[0]
        uppQueueSizeService = intQueueSizeService[1]
        val intQueueSizeToT3 = confidenceInterval(queueSizeToT3)
        lowQueueSizeToT3 = intQueueSizeToT3[0]
        uppQueueSizeToT3 = intQueueSizeToT3[1]
        val intWaitTimeT1 = confidenceInterval(waitTimeT1)
        lowWaitTimeT1 = intWaitTimeT1[0]
        uppWaitTimeT1 = intWaitTimeT1[1]
        val intWaitTimeT2 = confidenceInterval(waitTimeT2)
        lowWaitTimeT2 = intWaitTimeT2[0]
        uppWaitTimeT2 = intWaitTimeT2[1]
        val intWaitTimeService = confidenceInterval(waitTimeService)
        lowWaitTimeService = intWaitTimeService[0]
        uppWaitTimeService = intWaitTimeService[1]
        val intWaitTimeToT3 = confidenceInterval(waitTimeToT3)
        lowWaitTimeToT3 = intWaitTimeToT3[0]
        uppWaitTimeToT3 = intWaitTimeToT3[1]
    }

    fun totalCosts() = driveCosts + driverCosts + employeeCosts
}