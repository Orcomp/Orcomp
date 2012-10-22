# Interval

## Purpose

- Well documented library for dealing with Intevals and their EndPoints.
- Nomenclature taken from accepted papers (mainly http://web.cacs.louisiana.edu/~logan/521_f08/Doc/p832-allen.pdf)

## Introduction

An interval always has two EndPoints. One at each extremum.

The left EndPoint is called the Min.
The right EndPoint is called the Max.

An EndPoint is aware of the interval it belongs to.
(i.e. An EndPoint can only exist if there is an interval.)

## Methods

**Overlaps**: Returns true if two invervals overlap each other

**Meets**: Return true if the EndPoint of two intervals meet each other.

## DataStructures

- IntervalTree

## Notes

The main purpose of using intervals is to query their relationships (most noteably whether they overlap or not)

Starts Before Start, Ends before End
> +-----------+
>     +------------+

Starts Before Start, Ends after End (Contains)
> +---------------+
>     +------+


Starts After Start, Ends before End (During, or IsContained)
>     +------------+
>   +-----------------+

Starts After End
>              +----------+
>  +-----+

Ends Before Start
> +------+
>           +-------+

Starts At Start, Ends at End
>  +---------+
>  +---------+

# EndPoint

EndPoints can be inclusive or exclusive of their value.
EndPoints can also represent +/- infinity by specidying double.PositiveInfinity or double.NegativeInfinity


# Links

- Maintaining Knowledge about Temporal Intervals (http://web.cacs.louisiana.edu/~logan/521_f08/Doc/p832-allen.pdf)