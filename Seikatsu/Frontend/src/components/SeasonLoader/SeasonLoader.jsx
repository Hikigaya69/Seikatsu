import { useEffect, useState } from "react";

const seasons = [
    { name: "Spring", color: "bg-pink-200", text: "🌸 Preparing freshness..." },
    { name: "Summer", color: "bg-blue-200", text: "☀️ Warming things up..." },
    { name: "Autumn", color: "bg-yellow-200", text: "🍁 Gathering goodness..." },
    { name: "Winter", color: "bg-gray-200", text: "❄️ Almost ready..." },
];

export default function SeasonLoader() {
    const [index, setIndex] = useState(0);

    useEffect(() => {
        const interval = setInterval(() => {
            setIndex((prev) => (prev + 1) % seasons.length);
        }, 1200); // change every 1.2s

        return () => clearInterval(interval);
    }, []);

    const current = seasons[index];

    return (
        <div className="flex flex-col items-center justify-center h-60 gap-4">

            {/* Animated circle */}
            <div
                className={`w-8 h-8 rounded ${current.color} 
           duration-700 animate-spin`}
            ></div>

            {/* Text */}
            <p className="text-gray-700 text-xl">
                {current.text}
            </p>
        </div>
    );
}