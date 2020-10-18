package sk.ikim23.aircarrental.gui.model

import javafx.beans.property.SimpleDoubleProperty
import javafx.beans.property.SimpleIntegerProperty
import sk.ikim23.aircarrental.sim.safeDiv
import sk.ikim23.aircarrental.stat.SimStats

class GraphDataModel(nEmployees: Int, nBuses: Int, stats: SimStats) {
    val nEmployees = SimpleIntegerProperty(nEmployees)
    val nBuses = SimpleIntegerProperty(nBuses)
    val avgSystemTimeArrivingPassenger = SimpleDoubleProperty(stats.avgSystemTimeArrivingPassenger safeDiv 60)
    val lowSystemTimeArrivingPassenger = SimpleDoubleProperty(stats.lowSystemTimeArrivingPassenger safeDiv 60)
    val uppSystemTimeArrivingPassenger = SimpleDoubleProperty(stats.uppSystemTimeArrivingPassenger safeDiv 60)
    val avgSystemTimeLeavingPassenger = SimpleDoubleProperty(stats.avgSystemTimeLeavingPassenger safeDiv 60)
    val lowSystemTimeLeavingPassenger = SimpleDoubleProperty(stats.lowSystemTimeLeavingPassenger safeDiv 60)
    val uppSystemTimeLeavingPassenger = SimpleDoubleProperty(stats.uppSystemTimeLeavingPassenger safeDiv 60)
    val totalCosts = SimpleDoubleProperty(stats.totalCosts())

    init {
        val str = arrayOf(
                nEmployees,
                nBuses,
                avgSystemTimeArrivingPassenger.get(),
                lowSystemTimeArrivingPassenger.get(),
                uppSystemTimeArrivingPassenger.get(),
                avgSystemTimeLeavingPassenger.get(),
                lowSystemTimeLeavingPassenger.get(),
                uppSystemTimeLeavingPassenger.get(),
                totalCosts.get()

        ).joinToString("\t")
        println(str)
    }
}