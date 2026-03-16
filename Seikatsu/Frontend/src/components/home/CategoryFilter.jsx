export default function CategoryFilter() {

  const categories = [
    "All",
    "Groceries",
    "Snacks",
    "Drinks",
    "Ready Meals"
  ];

  return (

    <div className="flex gap-4 py-8">

      {categories.map((cat) => (

        <button
          key={cat}
          className="px-4 py-2 bg-gray-200 rounded-lg hover:bg-[#284b63] hover:text-white"
        >
          {cat}
        </button>

      ))}

    </div>

  );
}