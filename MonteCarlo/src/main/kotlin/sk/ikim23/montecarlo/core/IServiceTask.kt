package sk.ikim23.montecarlo.core

interface IServiceTask<T> {
    fun initialize()
    fun hasNext(): Boolean
    fun tick(): T
}