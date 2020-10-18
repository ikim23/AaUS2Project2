package sk.ikim23.montecarlo.model

import javafx.beans.property.*
import sk.ikim23.montecarlo.core.Status
import tornadofx.*

class RenderControlsModel(private val statusProperty: ObjectProperty<Status>, reps: Int, maxPts: Int) {
    val replicationsProperty = SimpleIntegerProperty(reps)
    val maxPointsProperty = SimpleIntegerProperty(maxPts)
    private val startDisable = SimpleBooleanProperty()
    private val pauseDisable = SimpleBooleanProperty()
    private val stopDisable = SimpleBooleanProperty()

    init {
        statusProperty.onChange { update() }
        replicationsProperty.onChange { update() }
        maxPointsProperty.onChange { update() }
        update()
    }

    fun startDisableProperty(): ReadOnlyBooleanProperty = startDisable
    fun pauseDisableProperty(): ReadOnlyBooleanProperty = pauseDisable
    fun stopDisableProperty(): ReadOnlyBooleanProperty = stopDisable
    fun skipPoints() = ((1.0 / maxPointsProperty.value) * replicationsProperty.value).toInt()

    private fun update() {
        val validData = 0 < maxPointsProperty.value && maxPointsProperty.value < replicationsProperty.value
        if (validData) {
            when (statusProperty.value) {
                Status.RUNNING -> {
                    startDisable.set(true)
                    pauseDisable.set(false)
                    stopDisable.set(false)
                }
                Status.PAUSED -> {
                    startDisable.set(false)
                    pauseDisable.set(true)
                    stopDisable.set(false)
                }
                else -> {
                    startDisable.set(false)
                    pauseDisable.set(true)
                    stopDisable.set(true)
                }
            }
        } else {
            startDisable.set(true)
            pauseDisable.set(true)
            stopDisable.set(true)
        }
    }
}