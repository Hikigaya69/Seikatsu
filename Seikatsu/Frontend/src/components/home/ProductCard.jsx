export default function ProductCard({ product }) {

  return (

    <div className="bg-white shadow rounded-lg p-4 hover:shadow-lg transition">

      <img
        src={product.productImageUrl}
        className="h-48 w-full object-cover rounded"
      />

      <h3 className="mt-4 font-semibold">
        {product.name}
      </h3>

      <p className="text-gray-500 text-sm">
        {product.description}
      </p>

      <div className="flex justify-between items-center mt-4">

        <span className="text-[#284b63] font-bold">
          ¥{product.price}
        </span>

        <button className="bg-[#3c6e71] text-white px-3 py-1 rounded">
          View Details
        </button>

      </div>

    </div>

  );
}