import { useState, useEffect } from 'react';
import ProductionLine from './ProductionLine';
import useSnapshot from '../hooks/useSnapshot';
import useLiveProduction from '../hooks/useLiveProduction';

const ProductionSection = () => {
    const {data: snapshotData, loading, error: snapshotError} = useSnapshot();
    const { series: liveSeries, error: liveError } = useLiveProduction();

    const series = liveSeries ?? snapshotData?.production;
    const error = liveError ?? snapshotError;

    if (error) {
        return (
            <section className="card card-production">
                <h4 className="section-title">Produktionsstatus</h4>
               <p>Fel: {error}</p>
            </section>
        )
    
    }

    if (loading || !series || !series.labels || !series.lines) {
        return (
            <section className="card card-production">
                <h4 className="section-title">Produktionsstatus</h4>
                <p>Laddar produktionsstatus...</p>
            </section>
            
        ) 
    } 

    return (
        <section className="card card-production">
            <h4 className="section-title">Produktionsstatus
                {import.meta.env.VITE_DEMO_MODE === "true" ? (
                    <span className="live-indicator">DEMO</span>
                ) : (
                    liveSeries && <span className="live-indicator">LIVE 🔴</span>
                )}
            </h4>
            
            <div className="chart-container">
                <ProductionLine series={series} />
               
            </div>
        </section>
    )
}

export default ProductionSection;