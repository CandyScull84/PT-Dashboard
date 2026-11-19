// import React from "react";
import { hexToRgba } from "../Utils/colorUtils";

const EndOfDayStat = ({ percent, label, color}) => {
    //Storlek o geometri för den cirkulära indikatorn
    const size = 90;
    const stroke = 10;
    const radius = (size-stroke) /2;
    const circumference = 2 * Math.PI * radius;
    const offset = circumference * (1 - percent/100);

    const trackColor = hexToRgba(color, 0.3);
    const barColor = hexToRgba(color, 1);

    return (
        <div className="eod-stat">
            <div className="eod-stat__label">{label}</div>
            <svg className="eod-stat__svg" width={size} height={size} viewBox={ `0 0 ${size} ${size}`} role="img" aria-label={`${label}: ${percent}% avslutat`}>
                <circle className="eod-stat__track" cx={size/2} cy={size/2} r={radius} strokeWidth={stroke} fill="none" stroke={trackColor} />
                <circle className="eod-stat__bar" cx={size/2} cy={size/2} r={radius} strokeWidth={stroke} fill="none" stroke={barColor} strokeDasharray={circumference} strokeDashoffset={offset} strokeLinecap="round" />
                <text x="50%" y="50%" textAnchor="middle" dominantBaseline="middle" className="eod-stat__text">
                    <tspan x="50%" dy="-0.4em" className="eod-stat__text--label">Avslutat</tspan>
                    <tspan x="50%" dy="1.2em" className="eod-stat__text--value">{percent}% </tspan>
                </text>
            </svg>          
        </div>
    );
};


export default EndOfDayStat;