package sk.ikim23.montecarlo.controllers

import javafx.beans.binding.Bindings
import javafx.beans.property.SimpleBooleanProperty
import javafx.beans.property.SimpleStringProperty
import javafx.scene.chart.XYChart
import sk.ikim23.montecarlo.core.*
import sk.ikim23.montecarlo.model.*
import sk.ikim23.montecarlo.problem.*
import tornadofx.*
import java.util.concurrent.Callable

typealias XYData = XYChart.Data<Number, Number>

class MainController : Controller() {
    private val core = RenderCore()
    private val keepGuessSeries = XYChart.Series<Number, Number>()
    private val changeGuessSeries = XYChart.Series<Number, Number>()
    val chartData = listOf(keepGuessSeries, changeGuessSeries).observable()
    val render = RenderControlsModel(core.statusProperty, 1_000_000, 1_000)
    val graph = GraphControlsModel(3)
    val simRunningProperty = SimpleBooleanProperty()
    val keepGuessValueProperty = SimpleStringProperty(0.toString())
    val changeGuessValueProperty = SimpleStringProperty(0.toString())

    init {
        keepGuessSeries.name = "Keep Guess"
        changeGuessSeries.name = "Change Guess"
        simRunningProperty.bind(Bindings.createBooleanBinding(Callable<Boolean> { core.statusProperty.value != Status.STOPPED }, core.statusProperty))
    }

    fun start() {
        if (core.statusProperty.value == Status.STOPPED) {
            clear()
            val reps = render.replicationsProperty.value
            val doors = graph.doorsProperty.value
            val skipPoints = render.skipPoints()
            if (graph.keepGuessVisibleProperty.value) {
                core.registerService(Service(KeepGuessTask(reps, doors), object : IResultRenderer<XYData> {
                    override fun render(results: List<XYData>) {
                        keepGuessSeries.data.addAll(results)
                        val last = results.lastOrNull()
                        if (last != null) keepGuessValueProperty.set(last.yValue.toString())
                    }
                }, skipPoints))
            }
            if (graph.changeGuessVisibleProperty.value) {
                core.registerService(Service(ChangeGuessTask(reps, doors), object : IResultRenderer<XYData> {
                    override fun render(results: List<XYData>) {
                        changeGuessSeries.data.addAll(results)
                        val last = results.lastOrNull()
                        if (last != null) changeGuessValueProperty.set(last.yValue.toString())
                    }
                }, skipPoints))
            }
        }
        core.start()
    }

    fun pause() {
        core.pause()
    }

    fun stop() {
        core.stop()
        clear()
    }

    private fun clear() {
        keepGuessSeries.data.clear()
        changeGuessSeries.data.clear()
        keepGuessValueProperty.set(0.toString())
        changeGuessValueProperty.set(0.toString())
        core.clear()
    }
}