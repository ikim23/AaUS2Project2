package sk.ikim23.carrental.model

import javafx.beans.property.SimpleDoubleProperty
import javafx.beans.property.SimpleIntegerProperty

class TableModel(nBus: Int, nEmpl: Int, low: Double, avg: Double, upp: Double) {
    val nBus = SimpleIntegerProperty(nBus)
    val nEmployees = SimpleIntegerProperty(nEmpl)
    val lowerBound = SimpleDoubleProperty(low)
    val average = SimpleDoubleProperty(avg)
    val upperBound = SimpleDoubleProperty(upp)
}