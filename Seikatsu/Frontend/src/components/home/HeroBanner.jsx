import heroBg from "../../assets/auth-bg.jpg";

export default function HeroBanner() {
  return (
    <div
      className="relative h-130 bg-cover bg-center flex items-center"
      style={{ backgroundImage: `url(${heroBg})` }}
    >

      
      <div className="absolute inset-0 bg-black/40"></div>

      {/* Content */}
      <div className="relative max-w-7xl mx-auto px-6 text-white">

        <h1 className="text-5xl font-bold leading-tight">
          Discover Global Groceries in Japan
        </h1>

        <p className="mt-4 text-lg max-w-xl">
          Shop authentic foods from India, Korea and around the world.
          Delivered fresh to your doorstep anywhere in Japan.
        </p>

        {/* Buttons */}
        <div className="mt-6 flex gap-4">

          <button className="bg-[#284b63] px-6 py-3 rounded-lg font-medium hover:bg-[#1f3a4d] transition">
            Shop Now
          </button>

          <button className="border border-white px-6 py-3 rounded-lg hover:bg-white hover:text-black transition">
            Browse Categories
          </button>

        </div>

      </div>
    </div>
  );
}