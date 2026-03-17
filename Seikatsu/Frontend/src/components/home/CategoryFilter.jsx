export default function CategoryFilter({ categories, selected, onSelect }) {

  return (

    <div className="flex gap-4 py-8 flex-wrap">

      {categories.map((cat) => (

        <button
          key={cat}
          onClick={() => onSelect(cat)}
          className={`px-4 py-2 rounded-lg transition
          ${
            selected === cat
              ? "bg-[#284b63] text-white"
              : "bg-gray-200 hover:bg-[#284b63] hover:text-white"
          }`}
        >
          {cat}
        </button>

      ))}

    </div>

  );
}