import EndOfDayStat from "./EndOfDayStat";
import { deptById } from "../data/departments";
import useSnapshot from "../hooks/useSnapshot";

const EndOfDaySection = () => {
    const { data, loading, error } = useSnapshot();

    if(error) {
        return (
        <section className="card card-eod" id="Slutresultat">
            <h4 className="section-title">Slutresultat</h4>
            <p>Fel: {error}</p>
        </section>
        );
    }

    const stats = data?.endOfDay || [];

    if (loading || stats.length === 0) {
        return (
            <section className="card card-eod" id="Slutresultat">
                <h4 className="section-title">Slutresultat</h4>
                <p>Laddar...</p>
            </section>
        );
    }

    return (
        <section className="card card-eod" id="Slutresultat">
            <h4 className="section-title">Slutresultat</h4>
            <div className="eod-grid">
                {stats.map((s) => {
                    const dept = deptById.get(s.deptId);
                    return (
                    <div className="eod-item" key= {s.deptId}>
                        <EndOfDayStat percent={s.completedPercent} label={dept?.name ?? `Dept ${s.deptId}`} color={dept?.color} />
                        {/* <div className="eod-label">Avslutat</div> */}
                    </div>
                    );
                })}         
            </div>
        </section> 
    );
};


export default EndOfDaySection;