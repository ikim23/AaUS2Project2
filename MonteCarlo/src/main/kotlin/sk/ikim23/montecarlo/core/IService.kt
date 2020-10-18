package sk.ikim23.montecarlo.core

interface IService {
    fun render()
    fun start()
    fun pause()
    fun stop()
    fun isDone(): Boolean
}