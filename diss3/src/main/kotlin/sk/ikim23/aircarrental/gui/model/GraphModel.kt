package sk.ikim23.aircarrental.gui.model

import javafx.application.Platform
import javafx.collections.ObservableList
import javafx.scene.chart.XYChart
import tornadofx.*

class GraphModel {
    val arrivingPassengersBusChartData = mutableListOf<XYChart.Series<Number, Number>>().observable()
    val arrivingPassengersEmplChartData = mutableListOf<XYChart.Series<Number, Number>>().observable()
    val leavingPassengersBusChartData = mutableListOf<XYChart.Series<Number, Number>>().observable()
    val leavingPassengersEmplChartData = mutableListOf<XYChart.Series<Number, Number>>().observable()
    private val arrPassBusSeries = HashMap<Int, XYChart.Series<Number, Number>>()
    private val arrPassEmplSeries = HashMap<Int, XYChart.Series<Number, Number>>()
    private val leavPassBusSeries = HashMap<Int, XYChart.Series<Number, Number>>()
    private val leavPassEmplSeries = HashMap<Int, XYChart.Series<Number, Number>>()

    fun take(graphData: GraphDataModel) {
        Platform.runLater {
            val nEmpl = graphData.nEmployees.get()
            val nBus = graphData.nBuses.get()
            val arrivingAvgTime = graphData.avgSystemTimeArrivingPassenger.get()
            val leavingAvgTime = graphData.avgSystemTimeLeavingPassenger.get()

            addData(arrivingPassengersBusChartData, arrPassBusSeries, nEmpl, nBus, arrivingAvgTime, "$nEmpl empl.")
            addData(arrivingPassengersEmplChartData, arrPassEmplSeries, nBus, nEmpl, arrivingAvgTime, "$nBus buses")
            addData(leavingPassengersBusChartData, leavPassBusSeries, nEmpl, nBus, leavingAvgTime, "$nEmpl empl.")
            addData(leavingPassengersEmplChartData, leavPassEmplSeries, nBus, nEmpl, leavingAvgTime, "$nBus buses")
        }
    }

    private fun addData(chartData: ObservableList<XYChart.Series<Number, Number>>, series: HashMap<Int, XYChart.Series<Number, Number>>, key: Int, x: Int, y: Double, title: String) {
        val mySeries = if (series.containsKey(key)) {
            series[key]
        } else {
            val newSeries = XYChart.Series<Number, Number>()
            newSeries.name = title
            series.put(key, newSeries)
            chartData.add(newSeries)
            newSeries
        }!!
        mySeries.data.add(XYChart.Data(x, y))
    }

    fun clear() {
        Platform.runLater {
            arrivingPassengersBusChartData.clear()
            arrivingPassengersEmplChartData.clear()
            leavingPassengersBusChartData.clear()
            leavingPassengersEmplChartData.clear()
        }
        arrPassBusSeries.clear()
        arrPassEmplSeries.clear()
        leavPassBusSeries.clear()
        leavPassEmplSeries.clear()
    }
}