package sk.ikim23.aircarrental.stat

import sk.ikim23.aircarrental.sim.Config
import sk.ikim23.aircarrental.sim.MySimulation

class CostStats(mySim: MySimulation) {
    val drivenKm: Double
    val driveCosts: Double
    val driverCosts: Double
    val employeeCosts: Double
    val totalCosts: Double get() = driveCosts + driverCosts + employeeCosts

    init {
        if (mySim.currentTime() > Config.warmTime) {
            val time = mySim.currentTime() - Config.warmTime
            drivenKm = mySim.bus.buses.size * Config.secToKm(time)
            driveCosts = drivenKm * mySim.bus.buses.first().type.price
            val hours = time / 3600.0
            driverCosts = mySim.bus.buses.size * hours * Config.driverSalary
            employeeCosts = mySim.serviceDesk.workers.size * hours * Config.employeeSalary
        } else {
            drivenKm = .0
            driveCosts = .0
            driverCosts = .0
            employeeCosts = .0
        }
    }
}