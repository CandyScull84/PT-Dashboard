// import React, { useState, useEffect } from 'react';+
import { Doughnut} from 'react-chartjs-2';
import {Chart as ChartJS, ArcElement, Tooltip, Legend } from 'chart.js';
import { useMemo } from 'react';

ChartJS.register(ArcElement, Tooltip, Legend)

const OrdersDonut = ({ chartData }) => {

  const options = useMemo (() => ({
    plugins: { 
      legend: {display: false }
    },
    maintainAspectRatio: false,
    responsive: true,
  }), []);

  return (
    <Doughnut data={chartData} options={options} />
  ) 
      
}

export default OrdersDonut;



