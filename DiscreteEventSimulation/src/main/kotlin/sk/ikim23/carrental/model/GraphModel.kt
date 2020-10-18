package sk.ikim23.carrental.model

import javafx.application.Platform
import javafx.collections.FXCollections
import javafx.scene.chart.XYChart

class GraphModel {
    val busChartData = FXCollections.observableArrayList<XYChart.Series<Number, Number>>()
    val emplChartData = FXCollections.observableArrayList<XYChart.Series<Number, Number>>()
    val tableData = FXCollections.observableArrayList<TableModel>()
    private val busSeries = HashMap<Int, XYChart.Series<Number, Number>>()
    private val emplSeries = HashMap<Int, XYChart.Series<Number, Number>>()

    fun take(nBus: Int, nEmpl: Int, lowTime: Double, avgTime: Double, uppTime: Double) {
        Platform.runLater {
            val bSeries = if (busSeries.containsKey(nEmpl)) {
                busSeries[nEmpl]
            } else {
                val newSeries = XYChart.Series<Number, Number>()
                newSeries.name = "$nEmpl empl."
                busSeries.put(nEmpl, newSeries)
                busChartData.add(newSeries)
                newSeries
            }!!
            bSeries.data.add(XYChart.Data(nBus, avgTime))

            val eSeries = if (emplSeries.containsKey(nBus)) {
                emplSeries[nBus]
            } else {
                val newSeries = XYChart.Series<Number, Number>()
                newSeries.name = "$nBus buses"
                emplSeries.put(nBus, newSeries)
                emplChartData.add(newSeries)
                newSeries
            }!!
            eSeries.data.add(XYChart.Data(nEmpl, avgTime))

            tableData.add(TableModel(nBus, nEmpl, lowTime, avgTime, uppTime))
        }
    }

    fun clear() {
        Platform.runLater {
            busChartData.clear()
            emplChartData.clear()
            tableData.clear()
        }
        busSeries.clear()
        emplSeries.clear()
    }
}