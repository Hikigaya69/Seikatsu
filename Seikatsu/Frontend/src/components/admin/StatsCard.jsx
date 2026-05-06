export default function StatsCard({
  title,
  value,
  subtitle
}) {

  return (

    <div className="bg-white rounded-2xl p-6 shadow-sm border">

      <p className="text-gray-500 text-sm">
        {title}
      </p>

      <h2 className="text-3xl font-bold mt-2">
        {value}
      </h2>

      <p className="text-gray-400 text-sm mt-2">
        {subtitle}
      </p>

    </div>

  );

}